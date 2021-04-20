using ProjectGenerator.Models;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ProjectGenerator.Service
{
    public class ProjectService : CrudService<Project>
    {
        public static Expression<Func<Project, bool>> GetExpressionPredicateForList(string Search)
        {
            Expression<Func<Project, bool>> predicate = x => x.IsDeleted == false;
            if (!string.IsNullOrEmpty(Search))
            {
                Expression<Func<Project, bool>> other = y => y.Name.Contains(Search);
                predicate = PredicateBuilder.And<Project>(predicate, other);
            }
            return predicate;
        }
        public static string GetGetFieldName(int Column)
        {
            switch (Column)
            {
                case 0:
                    return "Name";
                default:
                    return "Description";

            }
        }

    }
    public class TableService : CrudService<Table>
    {
        public static Expression<Func<Table, bool>> GetExpressionPredicateForList(string Search, Guid IDProject)
        {
            Expression<Func<Table, bool>> predicate = x => x.IsDeleted == false && x.ProjectID== IDProject;
            if (!string.IsNullOrEmpty(Search))
            {
                Expression<Func<Table, bool>> other = y => y.Name.Contains(Search);
                predicate = PredicateBuilder.And<Table>(predicate, other);
            }
            return predicate;
        }
        public static string GetGetFieldName(int Column)
        {
            switch (Column)
            {
                case 0:
                    return "Name";
                default:
                    return "Description";

            }
        }

    }
    public class DataFieldService : CrudService<DataField>
    {
     

    }
    public class DataFieldForeignKeyService : CrudService<DataFieldForeignKey>
    {
        
    }
}