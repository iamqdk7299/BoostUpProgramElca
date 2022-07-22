using NHibernate;

namespace NHibernateCore
{
    /// <summary>
    /// Using this interceptor in the <see cref="UnitOfWork.UnitOfWorkProvider"/> means that
    /// the interceptor from the <see cref="ISessionFactory"/> should be used (if any).
    /// </summary>
    public sealed class NullInterceptor : EmptyInterceptor
    {
        /// <summary>
        /// A static instance for this interceptor.
        /// </summary>
        public new static readonly NullInterceptor Instance = new NullInterceptor();
    }
}