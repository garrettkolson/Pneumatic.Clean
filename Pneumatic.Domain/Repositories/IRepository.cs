using System.Linq.Expressions;

namespace Pneumatic.Domain.Repositories;

public interface IRepository
{
    Task<T?> GetById<T>(int id) where T : DomainModel;
    
    Task<T?> FirstOrDefault<T>(Expression<Func<T, bool>> predicate) where T : DomainModel;
    Task<TResult?> FirstOrDefaultTransformed<T, TResult>(Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> transformFunction) where T : DomainModel;
    
    Task<List<T>> ToList<T>(Expression<Func<T, bool>> predicate) where T : DomainModel;
    Task<List<TResult>> ToListTransformed<T, TResult>(Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> transformFunction) where T : DomainModel;
    
    Task<bool> Any<T>(Expression<Func<T, bool>> predicate);
    
    Task Add<T>(T entity) where T : DomainModel;
    Task Update<T>(T entity) where T : DomainModel;
    Task Delete<T>(T entity) where T : DomainModel;
}