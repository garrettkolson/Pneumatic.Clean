using Pneumatic.Clean.Domain.Events;

namespace Pneumatic.Clean.Domain;

public abstract class DomainModel
{
    public List<DomainEvent> Events { get; set; } = new();
}