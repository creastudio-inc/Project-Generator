using System;
using System.Linq.Expressions;

namespace Infrastructure
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> ToExpression();

        bool IsSatisfiedBy(T entity);
    }
}
