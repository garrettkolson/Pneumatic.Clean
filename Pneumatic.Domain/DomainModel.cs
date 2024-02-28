using Pneumatic.Domain.Events;

namespace Pneumatic.Domain;

public abstract class DomainModel
{
    public List<DomainEvent> Events { get; set; } = new();
}