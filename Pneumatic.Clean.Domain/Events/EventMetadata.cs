namespace Pneumatic.Clean.Domain.Events;

public class EventMetadata
{
    public DateTime Timestamp { get; set; }
    public string? OriginatingModule { get; set; }
    
    public EventMetadata() { }

    public EventMetadata(DateTime timestamp, string module)
    {
        Timestamp = timestamp;
        OriginatingModule = module;
    }
}