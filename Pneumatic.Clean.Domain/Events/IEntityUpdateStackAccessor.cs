using Pneumatic.Clean.Domain.Users;

namespace Pneumatic.Clean.Domain.Events;

public interface IEntityUpdateStackAccessor
{
    EntityUpdateEvent CurrentEvent { get; }
    AppUser OriginatingUser { get; }
    int StackDepth { get; }
    int MaxStackDepth { get; }
    bool HasExceededMaxDepth { get; }
    void SetCurrentEvent(EntityUpdateEvent update);
}
    
public class EntityUpdateStackAccessor : IEntityUpdateStackAccessor
{
    public EntityUpdateEvent CurrentEvent { get; private set; }

    public AppUser OriginatingUser => getOriginatingUser();
    public int StackDepth => getStackDepth();
    public int MaxStackDepth => EntityUpdateStackConstants.MaxStackDepth;
    public bool HasExceededMaxDepth => StackDepth > MaxStackDepth;
    public void SetCurrentEvent(EntityUpdateEvent update) =>
        CurrentEvent = update;

    private int getStackDepth()
    {
        var depth = 0;
        var currentEvent = CurrentEvent;
        while (currentEvent != null)
        {
            depth++;
            currentEvent = currentEvent.PreviousEvent;
        }
            
        return depth;
    }

    private AppUser getOriginatingUser()
    {
        var currentEvent = CurrentEvent;
        while (currentEvent?.PreviousEvent != null)
        {
            currentEvent = currentEvent.PreviousEvent;
        }
            
        return currentEvent?.User;
    }
}

public class EntityUpdateStackConstants
{
    public const int MaxStackDepth = 10;
}