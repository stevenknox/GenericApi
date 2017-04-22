using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GenericApi
{
    public interface IGenericService<T, Tid, TContext>
    {
        T Add(T entity);
        T AddOrUpdate(T entity);
        void Delete(Tid id);
        T FindById(Tid id, params Expression<Func<T, object>>[] includeProperties);
        List<T> GetAll(params Expression<Func<T, object>>[] includeProperties);
        List<T> Query(Func<T, bool> predicate, params Expression<Func<T, object>>[] includeProperties);
        T Update(T entity);
    }

    public interface IGenericService<T, TContext> : IGenericService<T, int, TContext>
    {
    }
}