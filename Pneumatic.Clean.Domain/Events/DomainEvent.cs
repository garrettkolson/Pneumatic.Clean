using System.Reflection.Metadata;

namespace Pneumatic.Clean.Domain.Events;

public class DomainEvent : EventArgs
{
    public DomainModel? Entity { get; set; }

    public DomainEvent(DomainModel entity)
    {
        Entity = entity;
    }
}