namespace Pneumatic.Clean.Domain.Events;

public interface IEventBusManager
{
    Task PublishDomainEvent<T>(T @event) where T : DomainEvent;
    Task SubscribeToEvent<T>(AsyncEventHandler<DomainEvent> handler) where T : DomainEvent;
    Task PublishEntityEvent(EntityUpdateEvent update, EventType type);
    Task SubscribeToEntity<T>(EventType type, AsyncEventHandler<EntityUpdateEvent> handler) where T : DomainModel;
}