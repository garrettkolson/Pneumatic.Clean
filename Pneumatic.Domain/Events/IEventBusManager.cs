namespace Pneumatic.Domain.Events;

public interface IEventBusManager
{
    Task Publish<T>(T entity, EventType type) where T : DomainModel;
    Task Subscribe<T>(EventType type, AsyncEventHandler<DomainEvent> handler) where T : DomainModel;
}