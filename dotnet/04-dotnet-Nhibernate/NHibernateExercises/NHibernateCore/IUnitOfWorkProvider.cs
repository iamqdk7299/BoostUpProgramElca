using System;
using System.Data;

namespace NHibernateCore
{
    public interface IUnitOfWorkProvider
    {
        IUnitOfWorkScope Provide();

        IUnitOfWorkScope Provide(UnitOfWorkScopeOption scopeOption);

        IUnitOfWorkScope Provide(IsolationLevel isolationLevel);

        IUnitOfWorkScope Provide(UnitOfWorkScopeOption scopeOption, IsolationLevel isolationLevel);

        void Perform(Action action);

        void Perform(Action action, UnitOfWorkScopeOption scopeOption);

        void Perform(Action action, IsolationLevel isolationLevel);

        void Perform(Action action, UnitOfWorkScopeOption scopeOption, IsolationLevel isolationLevel);

        T Perform<T>(Func<T> func);

        T Perform<T>(Func<T> func, UnitOfWorkScopeOption scopeOption);

        T Perform<T>(Func<T> func, IsolationLevel isolationLevel);

        T Perform<T>(Func<T> func, UnitOfWorkScopeOption scopeOption, IsolationLevel isolationLevel);
    }
}
