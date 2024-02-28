namespace Pneumatic.Domain.Events;

public interface IEventBusManager
{
    Task PublishEvent<T>(T @event) where T : DomainEvent;
    Task SubscribeToEvent<T>(AsyncEventHandler<DomainEvent> handler) where T : DomainEvent;
    Task PublishEntityEvent<T>(T entity, EventType type) where T : DomainModel;
    Task SubscribeToEntity<T>(EventType type, AsyncEventHandler<DomainEvent> handler) where T : DomainModel;
}