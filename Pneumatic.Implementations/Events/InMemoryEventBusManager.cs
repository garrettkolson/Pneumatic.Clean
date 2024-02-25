using System.Collections.Concurrent;
using System.Security;
using Pneumatic.Domain;
using Pneumatic.Domain.Events;

namespace Pneumatic.Implementations.Events;

public class InMemoryEventBusManager : IEventBusManager
{
    private readonly ConcurrentDictionary<Type, AsyncEventHandler<DomainEvent>> _addRegistry = new();
    private readonly ConcurrentDictionary<Type, AsyncEventHandler<DomainEvent>> _updateRegistry = new();
    private readonly ConcurrentDictionary<Type, AsyncEventHandler<DomainEvent>> _deleteRegistry = new();
    
    public async Task Publish<T>(T entity, EventType type) where T : DomainModel
    {
        if (!getRegistry(type).TryGetValue(typeof(T), out var typeEvent)) return;
        DomainEvent domainEvent = new(entity);
        await typeEvent.Invoke(this, domainEvent);
    }

    public async Task Subscribe<T>(EventType type, AsyncEventHandler<DomainEvent> handler) where T : DomainModel
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
            EventType.Delete => _deleteRegistry
        };
    }
}