using System.Linq.Expressions;

namespace Pneumatic.Clean.Domain.Repositories;

public interface IReadRepository
{
    Task<T?> GetById<T>(int id) where T : DomainModel;
    
    Task<T?> FirstOrDefault<T>(Expression<Func<T, bool>> predicate) where T : DomainModel;
    Task<TResult?> FirstOrDefaultTransformed<T, TResult>(Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> transformFunction) where T : DomainModel;
    
    Task<List<T>> List<T>(Expression<Func<T, bool>> predicate) where T : DomainModel;
    Task<List<TResult>> ListTransformed<T, TResult>(Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> transformFunction) where T : DomainModel;
    
    Task<bool> Any<T>(Expression<Func<T, bool>> predicate);
}