using Pneumatic.Clean.Domain.Events;

namespace Pneumatic.Clean.Domain;

public abstract class DomainModel
{
    public abstract int KeyId { get; set; }
    
    public List<DomainEvent> Events { get; set; } = new();
}