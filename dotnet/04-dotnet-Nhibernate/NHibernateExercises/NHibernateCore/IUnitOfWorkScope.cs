using NHibernate;
using System;

namespace NHibernateCore
{
    public interface IUnitOfWorkScope : IDisposable
    {
        ISession Session { get; }

        void Complete();

        /// <summary>
        /// Initializes a lazy many-to-one/one-to-may/many-to-many relation (i.e. a collection)
        /// </summary>
        void InitializeProxy(params object[] proxyObjects);

        /// <summary>
        /// Creates a proxy object with the given primary key.
        /// </summary>
        /// <typeparam name="T">The type of the entity of the proxy to create.</typeparam>
        /// <param name="key">The primary key of the entity.</param>
        /// <returns>An uninitialized proxy instance with the given primary key.</returns>
        T GetProxy<T>(object key) where T : IBaseEntity;
    }
}
