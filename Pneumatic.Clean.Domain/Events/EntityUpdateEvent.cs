using Pneumatic.Clean.Domain.Changes;
using Pneumatic.Clean.Domain.Users;

namespace Pneumatic.Clean.Domain.Events;

public class EntityUpdateEvent : EventArgs
{
    public int KeyId { get; set; }
    public EventMetadata? Metadata { get; set; }
    public object? Entity { get; set; }
    public Type? EntityType { get; set; }
    public List<ChangedField> Changes { get; set; } = new();
    public AppUser? User { get; set; }
    public AppUser? OriginatingUser { get; set; }
    public EntityUpdateEvent? PreviousEvent { get; set; }
    
    public EntityUpdateEvent()
    {
        Metadata = new EventMetadata();
    }

    // TODO: fix this ctor
    public EntityUpdateEvent(object entity)
    {
        Entity = entity;
        Metadata = new EventMetadata(DateTime.UtcNow, "");
    }

    public object? GetOldValueFor(string propertyName)
    {
        return Changes.Find(x => x.Name == propertyName)?.OldValue;
    }
        
    public object? GetNewValueFor(string propertyName)
    {
        return Changes.Find(x => x.Name == propertyName)?.NewValue; 
    }
        
    private AppUser? getOriginatingUser()
    {
        if (PreviousEvent == null) return User;
            
        var prevEvent = PreviousEvent;
        while (prevEvent != null)
        {
            if (prevEvent.PreviousEvent == null)
                return prevEvent.User;
                
            prevEvent = prevEvent.PreviousEvent;
        }
            
        return null;
    }
}