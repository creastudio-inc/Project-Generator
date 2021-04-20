 
using Infrastructure.Entity;
using ProjectGenerator.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks; 

namespace Infrastructure
{
    /// <summary>
    /// The Abstract Entity Framework Data Access Object, perform basic CRUD, paging query.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="Guid">The type of the id.</typeparam>
    public  class GenericRepository<T> where T : EntityBase
    {
        #region contructors




        public GenericRepository()
        {
            this._context = ApplicationDbContext.doGetDbContext();
            var objectContext = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)this._context).ObjectContext;

            this._entity = objectContext.CreateObjectSet<T>();
            this.EnableLazyLoading = false;
        }

        
        #endregion

        #region Properties

        #region EnableLazyLoading

        /// <summary>
        /// Gets or sets a value indicating whether [enable lazy loading].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable lazy loading]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableLazyLoading
        {
            get;
            set;
        }

        #endregion

        #region Context

        /// <summary>
        /// The context
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected ApplicationDbContext Context
        {
            get { return _context; }
        }

        #endregion

        #region Entity

        /// <summary>
        /// The entity
        /// </summary>
        private readonly ObjectSet<T> _entity;
        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <value>
        /// The entity.
        /// </value>
        protected IObjectSet<T> Entity
        {
            get { return _entity; }
        }

        #endregion

        #endregion

        #region Protected Methods

        #region Initialize

        /// <summary>
        /// Call this method before each query
        /// </summary>
        protected virtual void Initialize()
        {
            this.Context.ObjectContext.ContextOptions.LazyLoadingEnabled = this.EnableLazyLoading;
        }

        #endregion

        #region LoadProperties

        /// <summary>
        /// for loading navigation properties
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="properties">The properties.</param>
        protected virtual void LoadProperties(object entity, params string[] properties)
        {
            foreach (string p in properties)
                this.Context.ObjectContext.LoadProperty(entity, p);
        }

        #endregion

        #region GetEntityName

        /// <summary>
        /// Get the entity name of the T
        /// </summary>
        /// <returns></returns>
        protected virtual string GetEntityName()
        {
            return string.Format("{0}.{1}", _entity.EntitySet.EntityContainer, _entity.EntitySet.Name);
        }

        #endregion

        #region PerformPaging

        /// <summary>
        /// Performs the paging.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortings">The sortings.</param>
        /// <returns></returns>
        protected Tuple<IQueryable<T>, int> PerformPaging(IQueryable<T> query
            , int pageIndex
            , int pageSize
            , List<SortDescriptor> sortings)
        {
            int index = 0;

            foreach (var sorting in sortings)
            {
                if (string.IsNullOrWhiteSpace(sorting.Field))
                    continue;

                if (sorting.Direction == SortDescriptor.SortingDirection.Ascending)
                {
                    query = CallMethod(query, index == 0 ? "OrderBy" : "ThenBy", sorting.Field);
                    index++;
                }
                else if(sorting.Direction == SortDescriptor.SortingDirection.Descending)
                {
                    query = CallMethod(query, index == 0 ? "OrderByDescending" : "ThenByDescending", sorting.Field);
                    index++;
                }
            }
            
            var totalRowCount = query.Count<T>();

            if (pageIndex > 0 && pageSize > 0)
                 query = query.Skip<T>(pageSize* (pageIndex - 1) ).Take( pageSize);

            else if (pageIndex == 0 && pageSize > 0)
                query = query.Take<T>(pageSize);

            return new Tuple<IQueryable<T>, int>(query, totalRowCount);
        }

 
        #endregion

        #region CallMethod

        /// <summary>
        /// Calls the method.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <returns></returns>
        protected IOrderedQueryable<T> CallMethod(IQueryable<T> query, string methodName, string memberName)
        {
            var typeParams = new ParameterExpression[] { Expression.Parameter(typeof(T), "") };

            System.Reflection.PropertyInfo pi = typeof(T).GetProperty(memberName);

            return (IOrderedQueryable<T>)query.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { typeof(T), pi.PropertyType },
                    query.Expression,
                    Expression.Lambda(Expression.Property(typeParams[0], pi), typeParams))
            );
        }

        #endregion

        #endregion

        #region IRepository<T> Members

        #region Add

        /// <summary>
        /// Creates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public virtual T doAddEntity(T entity)
        {
            var fqen = GetEntityName();

            this.Context.ObjectContext.AddObject(fqen, entity);
            this.Context.SaveChanges();
            this.Context.doUpdateAccessDbContext(false);
            return entity;
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public virtual T doUpdateEntity(T entity)
        {
            object originalItem;
            var fqen = GetEntityName();

           var key = this.Context.ObjectContext.CreateEntityKey(fqen, entity);

            if (this.Context.ObjectContext.TryGetObjectByKey(key, out originalItem))
            {
                this.Context.ObjectContext.ApplyCurrentValues(key.EntitySetName, entity);
            }
      
            this.Context.SaveChanges();
            this.Context.doUpdateAccessDbContext(false);
            return entity;
        }

        #endregion

        #region CreateOrUpdate

        /// <summary>
        /// Creates the or update.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public virtual T doCreateOrUpdate(T entity)
        {
            if (entity.IsTransient())
            {
                doAddEntity(entity);
            }
            else
            {
                doUpdateEntity(entity);
            }
            this.Context.doUpdateAccessDbContext(false);
            return entity;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public virtual void doDeleteEntity(Guid id)
        {
            var Entity = this.doGetEntityByID(id);
            Entity.IsDeleted = true;
            this.doUpdateEntity(Entity);
            this.Context.doUpdateAccessDbContext(false);

        }

        public virtual void doSoftDeleteEntity(Guid id)
        {
            this.Context.ObjectContext.DeleteObject(this.doGetEntityByID(id));
            this.Context.SaveChanges();
            this.Context.doUpdateAccessDbContext(false);

        }

        #endregion

        #region FindByKey

        /// <summary>
        /// Finds the by key.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns></returns>
        public virtual T doGetEntityByID(Guid id, params Expression<Func<T, object>>[] includeProperties)
        {
            var item = Expression.Parameter(typeof(T), "entity");
            var prop = Expression.Property(item, "Id");
            var value = Expression.Constant(id);
            var equal = Expression.Equal(prop, value);
            var lambda = Expression.Lambda<Func<T, bool>>(equal, item);

            var query = this.Entity.Where(lambda);

            if (includeProperties.Any())
            {
                query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }
            var result = query.SingleOrDefault();
            this.Context.doUpdateAccessDbContext(false);
            return result;
        }



        #endregion

        public virtual int doGetCount(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = PrepareFindQuery(predicate, includeProperties);

            var result = query.Count();
             this.Context.doUpdateAccessDbContext(false);
            return result;
        }


        #region Find

        /// <summary>
        /// Finds the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns></returns>
        public virtual IEnumerable<T> doFindEntity(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = PrepareFindQuery(predicate, includeProperties);
             
            var result = query.AsEnumerable();
            this.Context.doUpdateAccessDbContext(false);
            return result;
        }
 

        /// <summary>
        /// Finds the specified predicate.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns></returns>
        public virtual IEnumerable<T> doFindEntityBySpecification(ISpecification<T> spec, params Expression<Func<T, object>>[] includeProperties)
        {
            return doFindEntity(spec.ToExpression(), includeProperties);
        }

      


        #endregion

        #region FindOne

        /// <summary>
        /// Finds the one.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns></returns>
        public virtual T doGetEntity(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            Initialize();

            var entityName = GetEntityName();
            var query = this.Context.ObjectContext.CreateQuery<T>(entityName)
                                .Where(predicate);

            if (includeProperties.Any())
            {
                query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }
             
            var result = query.FirstOrDefault();
            this.Context.doUpdateAccessDbContext(false);
            return result;
        }

      
        /// <summary>
        /// Finds the one.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns></returns>
        public virtual T doGetEntity(ISpecification<T> spec, params Expression<Func<T, object>>[] includeProperties)
        {
            Initialize();

            return doGetEntity(spec.ToExpression(), includeProperties);
        }
        #endregion

        #region FindAll

        
        public virtual Tuple<IEnumerable<T>, int> doGetListEntity(Expression<Func<T, bool>> predicate
         , int pageIndex
         , int pageSize
         , List<SortDescriptor> sortings
         , params Expression<Func<T, object>>[] includeProperties)
        {
            var paged = PrepareFindQuery(predicate
            , pageIndex
            , pageSize
            , sortings
            , includeProperties);

            var query = paged.Item1;
            var totalRowCount = paged.Item2;

            return new Tuple<IEnumerable<T>, int>(query.ToList(), totalRowCount);
        }
        /// <summary>
        /// Finds the specified spec.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortings">The sortings.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns></returns>
        public virtual Tuple<IEnumerable<T>, int> doGetListEntity(ISpecification<T> spec
                                                        , int pageIndex
                                                        , int pageSize
                                                        , List<SortDescriptor> sortings
                                                        , params Expression<Func<T, object>>[] includeProperties)
        {
            return doGetListEntity(spec.ToExpression()
                        , pageIndex
                        , pageSize
                        , sortings
                        , includeProperties);
        } 
         
        #endregion
        
        #region GetQuery

        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> GetQuery()
        {
            Initialize();

            var entityName = GetEntityName();
            var query = this.Context.ObjectContext.CreateQuery<T>(entityName).AsQueryable();

            return query;
        }

        /// <summary>
        /// Prepares the find query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns></returns>
        protected virtual IQueryable<T> PrepareFindQuery(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            Initialize();

            var entityName = GetEntityName();

            var query = this.Context.ObjectContext.CreateQuery<T>(entityName).AsQueryable();
            if (predicate != null)
                query = query.Where(predicate);
            if (includeProperties.Any())
            {
                query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }

            return query;
        }

        /// <summary>
        /// Prepares the find query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortings">The sortings.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns></returns>
        public virtual Tuple<IQueryable<T>, int> PrepareFindQuery(Expression<Func<T, bool>> predicate
            , int pageIndex
            , int pageSize
            , List<SortDescriptor> sortings
            , params Expression<Func<T, object>>[] includeProperties)
        {
            Initialize();

            var entityName = GetEntityName();
            IQueryable<T> query = null;
            if (predicate!=null)
                query = this.Context.ObjectContext.CreateQuery<T>(entityName).Where(predicate);
            else
                query = this.Context.ObjectContext.CreateQuery<T>(entityName);

            if (sortings == null)
                sortings = new List<SortDescriptor>();
            
            var paged = PerformPaging(query, pageIndex, pageSize, sortings);
            query = paged.Item1;
            var totalRowCount = paged.Item2;

            if (includeProperties !=null && includeProperties.Any())
            {
                query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }

            return new Tuple<IQueryable<T>, int>(query, totalRowCount);
        }



        #endregion

        #endregion
    }
}
