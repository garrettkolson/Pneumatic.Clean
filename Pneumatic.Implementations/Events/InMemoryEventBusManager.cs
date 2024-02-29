using System.Collections.Concurrent;
using Pneumatic.Domain;
using Pneumatic.Domain.Events;

namespace Pneumatic.Implementations.Events;

public class InMemoryEventBusManager : IEventBusManager
{
    // TODO: write a way to configure if events should be dispatched concurrently or not
    
    private readonly ConcurrentDictionary<Type, AsyncEventHandler<DomainEvent>> _eventRegistry = new();
    
    private readonly ConcurrentDictionary<Type, AsyncEventHandler<DomainEvent>> _addRegistry = new();
    private readonly ConcurrentDictionary<Type, AsyncEventHandler<DomainEvent>> _updateRegistry = new();
    private readonly ConcurrentDictionary<Type, AsyncEventHandler<DomainEvent>> _deleteRegistry = new();

    public async Task PublishEvent<T>(T @event) where T : DomainEvent
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

    public async Task PublishEntityEvent<T>(T entity, EventType type) where T : DomainModel
    {
        if (!getRegistry(type).TryGetValue(typeof(T), out var typeEvent)) return;
        DomainEvent domainEvent = new(entity);
        await typeEvent.InvokeAsync(this, domainEvent);
    }

    public async Task SubscribeToEntity<T>(EventType type, AsyncEventHandler<DomainEvent> handler) where T : DomainModel
    {
        var registry = getRegistry(type);
        if (!registry.TryGetValue(typeof(T), out var typeHandler))
            registry.TryAdd(typeof(T), handler);
        else
            typeHandler += handler;
    }

    private ConcurrentDictionary<Type, AsyncEventHandler<DomainEvent>> getRegistry(EventType eventType)
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