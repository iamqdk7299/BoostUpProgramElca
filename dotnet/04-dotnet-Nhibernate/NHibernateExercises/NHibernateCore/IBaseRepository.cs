using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NHibernateCore
{
    public interface IBaseRepository<T> where T : class
    {
        void Add(T instance);

        T Find(Expression<Func<T, bool>> filter);

        IList<T> FindAll(Expression<Func<T, bool>> filter);

        T Load(int id);

        IList<T> LoadAll(IList<int> ids);

        void SaveOrUpdate(T entity);

        void SaveOrUpdate(IList<T> entities);

        void Delete(T entity);

        void Delete(IList<T> entities);

        T Merge(T entity);

        T1 MergeOther<T1>(T1 entity) where T1 : class;

        IList<T> GetAll();

        T GetById(int id);

        T1 GetOtherById<T1>(int id) where T1 : class;

        IList<T> FindByCriteria(QueryOver<T, T> criteria, int maxResults);
    }
}
