namespace Pneumatic.Domain.Events;

public interface IEventBusManager
{
    Task Publish<T>(T model, EventType type) where T : DomainModel;
    Task Subscribe<T>(EventType type) where T : DomainModel;
}