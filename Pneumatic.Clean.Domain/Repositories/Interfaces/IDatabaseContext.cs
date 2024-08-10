using System.Linq.Expressions;
using Pneumatic.Clean.Domain.Changes;

namespace Pneumatic.Clean.Domain.Repositories;

// TODO: be sure to implement SemaphoreSlim around any EF context access
// TODO: should this be implemented per aggregate? or per domain?
public interface IDatabaseContext
{
    Task<T?> GetById<T>(int id);

    Task<T?> FirstOrDefault<T>(Expression<Func<T, bool>> predicate);
    Task<TResult?> FirstOrDefaultTransformed<T, TResult>(Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> transformFunction);
    
    Task<List<T>> ToList<T>(Expression<Func<T, bool>> predicate);
    Task<List<TResult>> ToListTransformed<T, TResult>(Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> transformFunction);
    
    Task<bool> Any<T>(Expression<Func<T, bool>> predicate);
    
    Task<List<ChangedField>> Add<T>(T entity);
    Task<List<ChangedField>> Update<T>(T entity);
    Task<List<ChangedField>> Delete<T>(T entity);
}