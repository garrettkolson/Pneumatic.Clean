namespace Pneumatic.Domain.Repositories;

public interface IRepository
{
    Task<T> GetById<T>(int id) where T : DomainModel;
    Task Add<T>(T entity) where T : DomainModel;
    Task Update<T>(T entity) where T : DomainModel;
    Task Delete<T>(T entity) where T : DomainModel;
}