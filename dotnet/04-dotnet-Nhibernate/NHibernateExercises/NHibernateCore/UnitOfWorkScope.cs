
using NHibernate;
using System;
using System.Data;
using System.Threading;

namespace NHibernateCore
{
    public enum UnitOfWorkScopeOption
    {
        Required,
        RequiresNew
    }

    public class UnitOfWorkScope : IUnitOfWorkScope
    {
        /// <summary>
        /// Field similar to <see cref="System.Transactions.Transaction.Current" /> in order
        /// to get access to the transaction's root scope.
        /// </summary>
        private static readonly AsyncLocal<UnitOfWorkScope> CurrentScope = new AsyncLocal<UnitOfWorkScope>();

        private bool _complete;

        private bool _nhibernateControlsTransaction = true;

        private readonly ISessionFactory _factory;

        private readonly IInterceptor _interceptor;

        /// <summary>
        /// Saved 'UnitOfWorkScope.Current', represents a kind of stack frame.
        /// </summary>
        private UnitOfWorkScope _savedScope;


        /// <summary>Initializes a new instance of the <see cref="UnitOfWorkScope" /> class.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="System.Transactions.TransactionScopeOption.Required" />
        /// default for UnitOfWorkScope inheritance.
        /// </remarks>
        public UnitOfWorkScope(ISessionFactory factory, IInterceptor interceptor)
            : this(UnitOfWorkScopeOption.Required, factory, interceptor)
        {
        }

        /// <summary>Initializes a new instance of the
        /// <see cref="UnitOfWorkScope" /> class.
        /// with the specified requirements.
        /// </summary>
        /// <param name="scopeOption">
        /// An instance of the <see cref="System.Transactions.TransactionScopeOption" />
        /// enumeration that describes the transaction requirements associated
        /// with this transaction scope.
        /// </param>
        /// <param name="factory"></param>
        /// <param name="interceptor"></param>
        public UnitOfWorkScope(UnitOfWorkScopeOption scopeOption, ISessionFactory factory, IInterceptor interceptor)
            : this(scopeOption, IsolationLevel.ReadCommitted, factory, interceptor)
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="UnitOfWorkScope" /> class.
        /// with the specified requirements.
        /// </summary>
        /// <param name="scopeOption">
        /// An instance of the <see cref="System.Transactions.TransactionScopeOption" />
        /// enumeration that describes the transaction requirements associated
        /// with this transaction scope.
        /// </param>
        /// <param name="transactionOptions">
        /// A <see cref="System.Transactions.TransactionOptions" /> structure
        /// that describes the transaction options to use if a new transaction
        /// is created. If an existing transaction is used, the timeout value in
        /// this parameter applies to the transaction scope. If that time expires
        /// before the scope is disposed, the transaction is aborted.
        /// </param>
        /// <param name="factory"></param>
        /// <param name="interceptor"></param>
        public UnitOfWorkScope(UnitOfWorkScopeOption scopeOption,
            IsolationLevel transactionOptions, ISessionFactory factory, IInterceptor interceptor)
        {
            _factory = factory;
            _interceptor = interceptor;
            InitializeScopeLinking(scopeOption, transactionOptions);
        }

        /// <summary>
        /// Current property is read-only
        /// </summary>
        public static UnitOfWorkScope Current => CurrentScope.Value;

        /// <summary>
        /// Wrapped nhibernate session. Provides access to the underlying  <see cref="System.Transactions.TransactionScope"/>.
        /// </summary>
        public ISession Session { get; private set; }

        #region InitializeScope
        /// <summary>
        /// Initializes a new session (if not inherited from ambient transaction)
        /// and updates the linking to the root scope.
        /// </summary>
        private void InitializeScopeLinking(UnitOfWorkScopeOption scopeOption, IsolationLevel isolationLevel)
        {
            InitializeScope(IsRootUnitOfWorkScope() ? UnitOfWorkScopeOption.RequiresNew : scopeOption, isolationLevel);
        }

        /// <summary>
        /// Checks if this scope is a root unitofwork scope.
        /// </summary>
        private static bool IsRootUnitOfWorkScope()
        {
            return Current == null;
        }

        /// <summary>
        /// Initializes the current scope as a new child scope (= non root).
        /// </summary>
        private void InitializeScope(UnitOfWorkScopeOption scopeOption, IsolationLevel isolationLevel)
        {
            switch (scopeOption)
            {
                case UnitOfWorkScopeOption.Required:
                    // Inherit connection cache & keep current root scope
                    InheritSession();
                    PushScope(Current);
                    break;
                case UnitOfWorkScopeOption.RequiresNew:
                    // Create new connection cache & become new root scope
                    InitializeNewSession(isolationLevel);
                    PushScope(this);
                    break;
            }
        }

        private void InheritSession()
        {
            Session = Current.Session;
        }

        private void InitializeNewSession(IsolationLevel isolationLevel)
        {
            Session = _interceptor is NullInterceptor ? _factory.OpenSession() : _factory.WithOptions().Interceptor(_interceptor).OpenSession();
            BeginTransaction(isolationLevel);
        }
        #endregion InitializeScope
        #region PushPop
        /// <summary>
        /// Add <c>scope</c> on top of the scope calling stack. Will be removed during
        /// <c>Dispose()</c>.
        /// </summary>
        /// <param name="scope">Scope to be pushed on the calling stack.</param>
        /// <remarks>
        /// Stack structure may contain redundant frames, but this keeps a consistent
        /// stack access in the code.
        /// </remarks>
        private void PushScope(UnitOfWorkScope scope)
        {
            _savedScope = CurrentScope.Value;
            CurrentScope.Value = scope;
        }

        /// <summary>
        /// Pops the top most stack frame from the scope calling stack and drops
        /// its content because it's no longer needed.
        /// </summary>
        /// <remarks>
        /// This won't necessarily exchange the <c>UnitOfWorkScope.Current</c>.
        /// </remarks>
        private void PopAndDropScope()
        {
            CurrentScope.Value = _savedScope;
        }
        #endregion PushPop
        #region SessionHandling
        /// <summary>
        /// Complete Scope.
        /// </summary>
        private void CompleteSession()
        {
            try
            {
                PreCompleteSession();
                if (_complete && HasActiveTransaction())
                {
                    // only flush/commit if outermost scope voted for complete and transaction is still active (not terminated by an inner scope).
                    Session.Flush();
                    CommitTransaction();
                }
                PostCompleteSession();
            }
            finally
            {
                // Close is not ambient transaction aware, so use dispose instead.
                try
                {
                    Session.Dispose();
                }
                catch (Exception)
                {
                    // ignore exception here as it would shadow a real exception probably.
                }

                Session = null;
            }
        }

        private void BeginTransaction(IsolationLevel isolationLevel)
        {
            if (System.Transactions.Transaction.Current == null)
            {
                // No ambient transaction 
                Session.BeginTransaction(isolationLevel);
                _nhibernateControlsTransaction = true;
            }
            else
            {
                _nhibernateControlsTransaction = false;
            }
        }

        private void CommitTransaction()
        {
            if (!_nhibernateControlsTransaction)
            {
                return;
            }
            try
            {
                Session.Transaction.Commit();
            }
            finally
            {
                Session.Transaction.Dispose();
            }
        }

        private void RollbackTransaction()
        {
            if (!_nhibernateControlsTransaction)
            {
                return;
            }
            try
            {
                Session.Transaction.Rollback();
            }
            finally
            {
                Session.Transaction.Dispose();
            }
        }

        private bool HasActiveTransaction()
        {
            return _nhibernateControlsTransaction ?
                Session.Transaction != null && Session.Transaction.IsActive :
               System.Transactions.Transaction.Current != null && System.Transactions.Transaction.Current.TransactionInformation.Status == System.Transactions.TransactionStatus.Active;
        }

        /// <summary>
        /// Plugin method to execute code before clearing connection cache.
        /// </summary>
        protected virtual void PreCompleteSession()
        {
        }

        /// <summary>
        /// Plugin method to execute code after clearing connection cache.
        /// </summary>
        protected virtual void PostCompleteSession()
        {
        }
        #endregion SessionHandling
        #region Complete
        /// <summary>
        /// Indicates that all operations within the scope are completed successfully.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// This method has already been called once.
        /// </exception>
        public void Complete()
        {
            PreCompleteTransaction();
            _complete = true;
            PostCompleteTransaction();
        }

        public void InitializeProxy(params object[] proxyObjects)
        {
            if (Session != null)
            {
                if (proxyObjects != null)
                {
                    foreach (var proxy in proxyObjects)
                    {
                        NHibernateUtil.Initialize(proxy);
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("No session");
            }
        }

        /// <inheritdoc />
        public T GetProxy<T>(object key) where T : IBaseEntity
        {
            return Session.Load<T>(key);
        }

        /// <summary>
        /// Plugin method to execute code before unitofwork scope complete.
        /// </summary>
        protected virtual void PreCompleteTransaction()
        {
        }

        /// <summary>
        /// Plugin method to execute code  after unitofwork scope complete.
        /// </summary>
        protected virtual void PostCompleteTransaction()
        {
        }
        #endregion Complete
        #region Dispose
        /// <summary>
        /// Ends the transaction scope. Removes the frame from the scope calling stack...
        /// (a) and close the database connections if this scope was the root scope.
        /// (b) Also cleares the cache if the creation of a new transaction was suppressed
        /// or the scope (incl. child scopes) has an exclusive transaction.
        /// In other words: Do not clear the cache if the old current scope will stay alive.
        /// </summary>
        public void Dispose()
        {
            var oldCurrent = Current;
            PopAndDropScope(); // we first need to ensure to cleanup the unitofworkscope stack so that we don't keep a stale unitofworkscope in the thread. 
            // After PopAndDropScope, Current points to the parentscope of this scope. oldCurrent points to the state before PopAndDropScope.
            try
            {
                if (Session == null)
                {
                    return;
                }

                if (!_complete && HasActiveTransaction())
                {
                    // if an inner scope doesn't vote for complete, the transaction must be rolled back.
                    // note that only the transaction is rolled back here. The session dispose (close) is done by the scope who openend the session so that we don't close multiple times.
                    try
                    {
                        RollbackTransaction();
                    }
                    catch (Exception)
                    {
                        // ignore any exception here as Rollback is most likely caused by an exception in the code. Propagating the rollback exception
                        // would then shadow the real problem.
                    }
                }

                try
                {
                    // for a scope, which opened a session:
                    // - flush the session / commit transaction if all inner scope voted for complete (i.e. transaction still active)
                    // - dispose the session now (a scope which hasn't open the session must never dispose it independant of vote for completion).
                    if (Current == null)
                    {
                        // (a) root scope
                        CompleteSession();
                    }
                    else if (oldCurrent != Current)
                    {
                        // (b) RequiresNew
                        CompleteSession();
                    }
                }
                catch (StaleObjectStateException)
                {
                    throw new OptimisticVersionException();
                }
            }
            finally
            {
                // don't dispose session here as this must only be done by a scope which opened the session as otherwise all sessions would be disposed by inner scopes.
                Session = null;
            }
        }
        #endregion Dispose
    }
}
