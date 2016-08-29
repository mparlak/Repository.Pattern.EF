using LinqKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Repository.Pattern.EF.Repositories;
using Repository.Pattern.EF.Extensions;
using Repository.Pattern.EF.UnitOfWork;

namespace Repository.Pattern.EF.Factory
{
    public class Repository<TEntity> : IRepositoryAsync<TEntity> where TEntity : class
    {
        #region Private Fields

        private readonly DbContext _context;
        private readonly DbSet<TEntity> _dbSet;
        private readonly IUnitOfWorkAsync _unitOfWork;

        #endregion Private Fields

        public Repository(DbContext context, UnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            if (_context != null)
            {
                _dbSet = _context.Set<TEntity>();
            }
        }

        public virtual TEntity Find(params object[] keyValues) => _dbSet.Find(keyValues);

        public TEntity Find(Expression<Func<TEntity, bool>> predicate) => _dbSet.FirstOrDefault(predicate);

        public TEntity Find<TKey>(Expression<Func<TEntity, TKey>> sortExpression, bool isDesc, Expression<Func<TEntity, bool>> predicate) => isDesc ? _dbSet.OrderBy(sortExpression).FirstOrDefault(predicate) : _dbSet.OrderByDescending(sortExpression).FirstOrDefault(predicate);

        public virtual IQueryable<TEntity> SelectQuery(string query, params object[] parameters) => _dbSet.FromSql(query, parameters).AsQueryable();

        public virtual void Insert(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Added;
            _dbSet.Add(entity);
            _unitOfWork.SyncObjectState(entity);
        }

        public virtual void InsertRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Insert(entity);
            }
        }

        public virtual void Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _dbSet.Attach(entity);
            _unitOfWork.SyncObjectState(entity);
        }

        public virtual void Delete(object id)
        {
            var entity = _dbSet.Find(id);
            Delete(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            _dbSet.Attach(entity);
            _unitOfWork.SyncObjectState(entity);
        }

        public virtual IQueryable<TEntity> GetAll() => _dbSet;

        public IQueryFluent<TEntity> Query() => new QueryFluent<TEntity>(this);

        public virtual IQueryFluent<TEntity> Query(IQueryObject<TEntity> queryObject) => new QueryFluent<TEntity>(this, queryObject);

        public virtual IQueryFluent<TEntity> Query(Expression<Func<TEntity, bool>> query) => new QueryFluent<TEntity>(this, query);

        public IQueryable<TEntity> Queryable() => _dbSet;

        public IRepository<T> GetRepository<T>() where T : class => _unitOfWork.Repository<T>();

        public virtual async Task<TEntity> FindAsync(params object[] keyValues) => await _dbSet.FindAsync(keyValues);

        public virtual async Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues) => await _dbSet.FindAsync(cancellationToken, keyValues);

        public virtual async Task<bool> DeleteAsync(params object[] keyValues) => await DeleteAsync(CancellationToken.None, keyValues);

        public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            var entity = await FindAsync(cancellationToken, keyValues);

            if (entity == null)
            {
                return false;
            }

            _context.Entry(entity).State = EntityState.Deleted;
            _context.Set<TEntity>().Attach(entity);
            _context.SaveChanges();
            return true;
        }

        internal IQueryable<TEntity> Select(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            int? page = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (predicate != null)
            {
                query = query.AsExpandable().Where(predicate);
            }
            if (page != null && pageSize != null)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }
            return query;
        }

        internal async Task<IEnumerable<TEntity>> SelectAsync(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            int? page = null,
            int? pageSize = null)
        {
            return await Select(predicate, orderBy, includes, page, pageSize).ToListAsync();
        }

        public virtual IQueryable<TEntity> Filter(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            int? page = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (predicate != null)
            {
                query = query.AsExpandable().Where(predicate);
            }
            if (page != null && pageSize != null)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }
            return query;
        }

        public virtual async Task<IEnumerable<TEntity>> FilterAsync(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            int? page = null,
            int? pageSize = null)
        {
            return await Select(predicate, orderBy, includes, page, pageSize).ToListAsync();
        }
    }
}
