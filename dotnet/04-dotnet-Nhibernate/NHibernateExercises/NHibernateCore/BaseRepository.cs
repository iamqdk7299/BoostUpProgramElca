using NHibernate;
using NHibernate.Criterion;
using NHibernate.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NHibernateCore
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected ISession Session
        {
            get
            {
                var session = UnitOfWorkScope.Session;
                if (session == null)
                {
                    throw new Exception("Session of the current UnitOfWorkScope is null");
                }
                return session;
            }
        }

        protected UnitOfWorkScope UnitOfWorkScope
        {
            get
            {
                var scope = UnitOfWorkScope.Current;
                if (scope == null)
                {
                    throw new Exception("Current UnitOfWorkScope is null");
                }
                return scope;
            }
        }

        public virtual void Add(T instance)
        {
            Session.Save(instance);
        }

        public T Find(Expression<Func<T, bool>> expression)
        {
            return FindAll(expression).FirstOrDefault();
        }

        public IList<T> FindAll(Expression<Func<T, bool>> filter)
        {
            return Session.QueryOver<T>().Where(filter).List();
        }

        public T Load(int id)
        {
            return Session.Load<T>(id);
        }

        public IList<T> LoadAll(IList<int> ids)
        {
            return ids?.Select(Load).ToList() ?? new List<T>();
        }

        public void SaveOrUpdate(T entity)
        {
            Session.SaveOrUpdate(entity);
        }

        public void SaveOrUpdate(IList<T> entities)
        {
            entities?.ForEach(SaveOrUpdate);
        }

        public void Delete(T entity)
        {
            Session.Delete(entity);
        }

        public void Delete(IList<T> entities)
        {
            entities?.ForEach(Delete);
        }

        public T Merge(T entity)
        {
            return Session.Merge(entity);
        }

        public T1 MergeOther<T1>(T1 entity) where T1 : class
        {
            return Session.Merge(entity);
        }

        public IList<T> GetAll()
        {
            return Session.QueryOver<T>().List();
        }

        public virtual T GetById(int id)
        {
            var criteria = Session.CreateCriteria<T>().Add(Property.ForName("Id").Eq(id));
            criteria.SetMaxResults(1);
            return criteria.UniqueResult() as T;
        }

        public virtual T1 GetOtherById<T1>(int id) where T1 : class
        {
            var criteria = Session.CreateCriteria<T1>().Add(Property.ForName("Id").Eq(id));
            criteria.SetMaxResults(1);
            return criteria.UniqueResult() as T1;
        }

        public IList<T> FindByCriteria(QueryOver<T, T> criteria, int maxResults)
        {
            if (maxResults > 0)
            {
                criteria.Take(maxResults);
            }
            var result = criteria.GetExecutableQueryOver(Session).List<T>();
            return result ?? Enumerable.Empty<T>().ToList();
        }
    }
}
