using Infrastructure;
using Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Service
{
    public class CrudService<T> where T : EntityBase
    {
        public static T doCreateOrUpdate(T model)
        {
            return new GenericRepository<T>().doCreateOrUpdate(model);
        }
        public static T doGetEntity(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            return new GenericRepository<T>().doGetEntity(predicate, includeProperties);
        }
        public static void doDeleteEntityByID(Guid id,bool SoftDelete=false)
        {
            if(SoftDelete==false)
                new GenericRepository<T>().doDeleteEntity(id);
            else
                new GenericRepository<T>().doSoftDeleteEntity(id);
        }
        public static Tuple<IEnumerable<T>, int> doGetListEntity(Expression<Func<T, bool>> predicate = null,  List<SortDescriptor> sortings = null, int pageIndex = 0, int pageSize = 0, params Expression<Func<T, object>>[] includeProperties)
        {
            return new GenericRepository<T>().doGetListEntity(predicate, pageIndex, pageSize, sortings, includeProperties);
        }
        public static int doGetCount(Expression<Func<T, bool>> predicate = null)
        {
            return new GenericRepository<T>().doGetCount(predicate);
        }
    }
}