using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ebor.Repository
{
    // This is a generic Repository - it can be used to hold any data type.
    // It's a repository in the truest sense - it only holds data in memory
    // You would need to add code to store the data somewhere - EF perhaps?
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity Get(int id);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);

    }

}
