using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Application.Interfaces.Repositories;
using Core.Context;

namespace Application.Repositories
{
    /// <summary>
    /// Abstract class that implements basic interface for IRepository&lt;T&gt;
    /// </summary>
    /// <typeparam name="T">Repository Entity type</typeparam>
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T: class
    {
        protected PlanningDbContext Context { get; set; }

        public RepositoryBase(PlanningDbContext Context)
        {
            this.Context = Context;
        }

        public IEnumerable<T> FindAll()
        {
            return Context.Set<T>();
        }

        public T Get(int id)
        {
            return Context.Set<T>().Find(id);
        }

        public IEnumerable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return Context.Set<T>().Where(expression).ToList();
        }

        public void Create(T entity)
        {
            Context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            Context.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            Context.Set<T>().Remove(entity);
        }

        public void Save()
        {
            Context.SaveChanges();
        }
    }
}
