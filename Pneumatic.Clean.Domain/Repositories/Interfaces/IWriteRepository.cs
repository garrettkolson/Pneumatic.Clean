using System.Linq.Expressions;

namespace Pneumatic.Clean.Domain.Repositories;

public interface IWriteRepository
{
    Task Add<T>(T entity) where T : DomainModel;
    Task Update<T>(T entity) where T : DomainModel;
    Task Delete<T>(T entity) where T : DomainModel;
}