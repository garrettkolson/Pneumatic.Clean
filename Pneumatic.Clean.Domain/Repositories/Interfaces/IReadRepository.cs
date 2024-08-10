using System.Linq.Expressions;

namespace Pneumatic.Clean.Domain.Repositories;

public interface IReadRepository<in TModelBase> where TModelBase : DomainModel
{
    Task<T?> GetById<T>(int id) where T : TModelBase;
    
    Task<T?> FirstOrDefault<T>(Expression<Func<T, bool>> predicate) where T : TModelBase;
    Task<TResult?> FirstOrDefaultTransformed<T, TResult>(Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> transformFunction) where T : TModelBase;
    
    Task<List<T>> List<T>(Expression<Func<T, bool>> predicate) where T : TModelBase;
    Task<List<TResult>> ListTransformed<T, TResult>(Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> transformFunction) where T : TModelBase;
    
    Task<bool> Any<T>(Expression<Func<T, bool>> predicate);
}