using log4net;
using log4net.Core;
using NHibernate;
using System;
using System.Data;

namespace NHibernateCore
{
    public class UnitOfWorkProvider : IUnitOfWorkProvider
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly IInterceptor _interceptor;

        /// <summary>
        /// Creates a new <see cref="UnitOfWorkProvider"/>.
        /// </summary>
        /// <param name="sessionFactory">The <see cref="ISessionFactory"/> used to create sessions.</param>
        /// <param name="logger">The logger that is used for logging, defaults to <see cref="NullLogger"/>.</param>
        /// <param name="interceptor">
        ///     The interceptor that is used for sessions. Defaults to <see cref="NullInterceptor"/> which means
        ///     that the interceptor from the <see cref="ISessionFactory"/> is used.
        /// </param>
        public UnitOfWorkProvider(ISessionFactory sessionFactory, IInterceptor interceptor = null)
        {
            _sessionFactory = sessionFactory ?? throw new NullReferenceException("SessionFactory cannot be null");
            _interceptor = interceptor ?? new NullInterceptor();
        }

        public IUnitOfWorkScope Provide()
        {
            return Provide(DetermineScopeOption());
        }

        public IUnitOfWorkScope Provide(IsolationLevel isolationLevel)
        {
            return Provide(DetermineScopeOption(), isolationLevel);
        }

        public IUnitOfWorkScope Provide(UnitOfWorkScopeOption scopeOption)
        {
            return Provide(scopeOption, IsolationLevel.ReadCommitted);
        }

        public IUnitOfWorkScope Provide(UnitOfWorkScopeOption scopeOption, IsolationLevel isolationLevel)
        {
            if (IsSessionFactoryOfAmbientScopeDifferent() && scopeOption == UnitOfWorkScopeOption.Required)
            {
                throw new ArgumentException("Ambient unit of work has incompatible session, please use RequiresNew to create a new unit of work.");
            }
            return new UnitOfWorkScope(scopeOption, isolationLevel, _sessionFactory, _interceptor);
        }

        public void Perform(Action action)
        {
            Perform(action, DetermineScopeOption(), IsolationLevel.ReadCommitted);
        }

        public void Perform(Action action, UnitOfWorkScopeOption scopeOption)
        {
            Perform(action, scopeOption, IsolationLevel.ReadCommitted);
        }

        public void Perform(Action action, IsolationLevel isolationLevel)
        {
            Perform(action, DetermineScopeOption(), isolationLevel);
        }

        public void Perform(Action action, UnitOfWorkScopeOption scopeOption, IsolationLevel isolationLevel)
        {
            using (var scope = Provide(scopeOption, isolationLevel))
            {
                action();
                scope.Complete();
            }
        }

        public T Perform<T>(Func<T> action)
        {
            return Perform(action, DetermineScopeOption(), IsolationLevel.ReadCommitted);
        }

        public T Perform<T>(Func<T> action, UnitOfWorkScopeOption scopeOption)
        {
            return Perform(action, scopeOption, IsolationLevel.ReadCommitted);
        }

        public T Perform<T>(Func<T> action, IsolationLevel isolationLevel)
        {
            return Perform(action, DetermineScopeOption(), isolationLevel);
        }

        public T Perform<T>(Func<T> action, UnitOfWorkScopeOption scopeOption, IsolationLevel isolationLevel)
        {
            T result;
            using (var scope = Provide(scopeOption, isolationLevel))
            {
                result = action();
                scope.Complete();
            }
            return result;
        }

        private UnitOfWorkScopeOption DetermineScopeOption()
        {
            var scopeOption = UnitOfWorkScopeOption.Required;
            if (IsSessionFactoryOfAmbientScopeDifferent())
            {
                scopeOption = UnitOfWorkScopeOption.RequiresNew;
            }
            return scopeOption;
        }

        private bool IsSessionFactoryOfAmbientScopeDifferent()
        {
            return UnitOfWorkScope.Current != null && UnitOfWorkScope.Current.Session != null &&
                _sessionFactory != UnitOfWorkScope.Current.Session.SessionFactory;
        }
    }
}
