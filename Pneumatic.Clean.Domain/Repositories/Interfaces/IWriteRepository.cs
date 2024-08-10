using System.Linq.Expressions;

namespace Pneumatic.Clean.Domain.Repositories;

public interface IWriteRepository<in TModelBase> where TModelBase : DomainModel
{
    Task Add<T>(T entity) where T : TModelBase;
    Task Update<T>(T entity) where T : TModelBase;
    Task Delete<T>(T entity) where T : TModelBase;
}