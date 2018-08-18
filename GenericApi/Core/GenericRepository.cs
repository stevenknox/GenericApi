using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace GenericApi
{
    public class GenericRepository<T, TContext> : IGenericRepository<T, TContext> where T: class where TContext: DbContext
    {

        protected readonly TContext _context;
        private DbSet<T> db;

        public GenericRepository(TContext context)
        {
            _context = context;
            db = context.Set<T>();
        }

        public List<T> GetAll(params Expression<Func<T, object>>[] includeProperties)
        {
           return AllIncluding(includeProperties).ToList();
        }

        public List<T> GetAll(string[] includeProperties)
        {
            return AllIncluding(includeProperties).ToList();
        }

        public T FindById(dynamic id, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = db.Find(id);

            foreach (var includeProperty in includeProperties)
            {
                if (includeProperty != null)
                    _context.Entry(query).Reference(includeProperty).Load();
            }

            return query;
        }

        public T FindById(dynamic id, string[] includeProperties)
        {
            var query = db.Find(id);

            foreach (var includeProperty in includeProperties)
            {
                if (includeProperty != null)
                    _context.Entry(query).Reference(includeProperty).Load();
            }

            return query;
        }

        public List<T> Query(Func<T, bool> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            return AllIncluding(includeProperties).Where(predicate).ToList();
        }

        public List<T> Query(Func<T, bool> predicate, string[] includeProperties)
        {
            return AllIncluding(includeProperties).Where(predicate).ToList();
        }

        private IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                if(includeProperty != null)
                    query = query.Include(includeProperty);
            }
            return query;
        }

        private IQueryable<T> AllIncluding(string[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                if (includeProperty != null)
                    query = query.Include(includeProperty);
            }
            return query;
        }

        public T Add(T entity)
        {
            (entity as dynamic).GenericApiState = GenericApiState.Added;
            return AddOrUpdate(entity);
        }

        public T Update(T entity)
        {
            (entity as dynamic).GenericApiState = GenericApiState.Modified;
            return AddOrUpdate(entity);
        }

        public T AddOrUpdate(T entity)
        {
            if (entity != null)
            {
                GenericApiState state = (entity as dynamic).GenericApiState;
                _context.ApplyStateChanges(entity, state);

                _context.SaveChanges();
                return entity;
            }
            else
            {
                throw new NullReferenceException($"Entity is null");
            }
        }

        public void Delete(dynamic id)
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
