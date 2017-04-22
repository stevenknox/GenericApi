using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace GenericApi
{
    public class GenericService<T> : GenericServiceBase<T, int>, IGenericService<T> where T : GenericEntity
    {
        public GenericService(DbContext context):base(context){ }
    }

    public class GenericServiceBase<T, Tid>: IGenericService<T, Tid> where T: GenericEntity
    {

        protected readonly DbContext _context;
        private DbSet<T> db;

        public GenericServiceBase(DbContext context)
        {
            _context = context;
            db = context.Set<T>();
        }

        public List<T> GetAll(params Expression<Func<T, object>>[] includeProperties)
        {
           return AllIncluding(includeProperties).ToList();
        }

        public T FindById(Tid id, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = db.Find(id);

            foreach (var includeProperty in includeProperties)
            {
                _context.Entry(query).Reference(includeProperty).Load();
            }

            return query;
        }

        public List<T> Query(Func<T, bool> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            return AllIncluding(includeProperties).Where(predicate).ToList();
        }

        private IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public T Add(T entity)
        {
            entity.EntityState = EntityState.Added;
            return AddOrUpdate(entity);
        }

        public T Update(T entity)
        {
            entity.EntityState = EntityState.Modified;
            return AddOrUpdate(entity);
        }

        public T AddOrUpdate(T entity)
        {
            if (entity != null)
            {
                _context.AddOrUpdate<T, Tid>(entity);
                _context.SaveChanges();
                return entity;
            }
            else
            {
                throw new NullReferenceException($"Entity is null");
            }
        }

        public void Delete(Tid id)
        {
            var entity = db.Find(id);
            if (entity != null)
            {
                _context.Remove(entity);
                _context.SaveChanges();
            }
            else
            {
                throw new NullReferenceException($"Entity with ID {id} not found");
            }
        }
    }
}
