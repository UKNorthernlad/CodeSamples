using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ebor.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        List<TEntity> w = new List<TEntity>();
        public void Add(TEntity entity)
        {
            w.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public TEntity Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Remove(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }
    }
}
