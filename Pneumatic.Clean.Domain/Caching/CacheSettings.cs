namespace Pneumatic.Clean.Domain.Caching;

public record CacheSettings
{
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
    public TimeSpan? SlidingExpiration { get; set; }
}