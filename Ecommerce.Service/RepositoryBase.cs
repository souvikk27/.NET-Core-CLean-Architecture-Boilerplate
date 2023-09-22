using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Service
{
    public class RepositoryBase<TEntity, TContext> where TEntity : class where TContext : DbContext
    {
        protected readonly TContext Context;

        public RepositoryBase(IRepositoryOptions<TContext> options)
        {
            Context = options.Context;
        }

        public virtual Expression<Func<TContext, DbSet<TEntity>>> DataSet() => null;
        public virtual Expression<Func<TEntity, object>> Key() => null;

        public virtual TEntity GetById(object id)
        {
            var keyExpression = Key();
            if (keyExpression == null)
            {
                throw new InvalidOperationException("Key expression is not defined for this repository.");
            }
            // Create a parameter expression for the entity
            var entityParameter = Expression.Parameter(typeof(TEntity), "entity");
            // Access the key property using the Key expression
            var keyAccessExpression = Expression.Invoke(keyExpression, entityParameter);
            var castedKey = Expression.Convert(keyAccessExpression, typeof(Guid));
            // Create a lambda expression for the predicate
            var lambda = Expression.Lambda<Func<TEntity, bool>>(
                Expression.Equal(castedKey, Expression.Constant(id)),
                entityParameter
            );
            // Use the lambda expression to retrieve the entity by ID
            return Context.Set<TEntity>().SingleOrDefault(lambda);
        }


        public virtual IEnumerable<TEntity> GetAll()
        {
            var entity = DataSet().Compile()(Context);
            return entity.ToList();
        }


        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate).ToList();
        }


        public virtual TEntity Add(TEntity entity)
        {
            EntityEntry<TEntity> entityEntry = Context.Set<TEntity>().Add(entity);
            return entityEntry.Entity;
        }


        public virtual TEntity Update(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            return entity;
        }


        public virtual void Remove(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
        }


        public virtual void Save()
        {
            Context.SaveChanges();
        }
    }
}
