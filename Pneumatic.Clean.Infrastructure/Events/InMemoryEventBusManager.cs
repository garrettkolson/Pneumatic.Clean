using System.Collections.Concurrent;
using Pneumatic.Clean.Domain;
using Pneumatic.Clean.Domain.Events;

namespace Pneumatic.Clean.Infrastructure.Events;

public class InMemoryEventBusManager : IEventBusManager
{
    // TODO: add logging
    // TODO: write a way to configure if events should be dispatched concurrently or not
    
    private readonly ConcurrentDictionary<Type, AsyncEventHandler<DomainEvent>> _eventRegistry = new();
    
    private readonly ConcurrentDictionary<Type, AsyncEventHandler<EntityUpdateEvent>> _addRegistry = new();
    private readonly ConcurrentDictionary<Type, AsyncEventHandler<EntityUpdateEvent>> _updateRegistry = new();
    private readonly ConcurrentDictionary<Type, AsyncEventHandler<EntityUpdateEvent>> _deleteRegistry = new();

    public async Task PublishDomainEvent<T>(T @event) where T : DomainEvent
    {
        if (!_eventRegistry.TryGetValue(typeof(T), out var handler)) return;
        await handler.InvokeAsync(this, @event);
    }

    public async Task SubscribeToEvent<T>(AsyncEventHandler<DomainEvent> handler) where T : DomainEvent
    {
        if (!_eventRegistry.TryGetValue(typeof(T), out var typeHandler))
            _eventRegistry.TryAdd(typeof(T), handler);
        else
            typeHandler += handler;
    }

    public async Task PublishEntityEvent(EntityUpdateEvent update, EventType type)
    {
        if (update.EntityType == null) return;
        if (!getRegistry(type).TryGetValue(update.EntityType, out var typeEvent)) return;
        await typeEvent.InvokeAsync(this, update);
    }

    public async Task SubscribeToEntity<T>(EventType type, AsyncEventHandler<EntityUpdateEvent> handler) where T : DomainModel
    {
        var registry = getRegistry(type);
        if (!registry.TryGetValue(typeof(T), out var typeHandler))
            registry.TryAdd(typeof(T), handler);
        else
            typeHandler += handler;
    }

    private ConcurrentDictionary<Type, AsyncEventHandler<EntityUpdateEvent>> getRegistry(EventType eventType)
    {
        return eventType switch
        {
            EventType.Add => _addRegistry,
            EventType.Update => _updateRegistry,
            EventType.Delete => _deleteRegistry,
            _ => throw new ArgumentException($"Event type of {eventType} is invalid.", nameof(eventType))
        };
    }
}