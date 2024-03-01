using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Memory;
using Pneumatic.Clean.Domain.Events;

namespace Pneumatic.Clean.Domain.Repositories;

// TODO: eventually configure this to use a RepoConfig (with cache settings, etc)
public abstract class RepositoryBase(
        IEventBusManager eventBus,
        IMemoryCache cache,
        IDatabaseContext databaseContext)
    : IReadRepository, IWriteRepository 
{
    public async Task<T?> GetById<T>(int id) where T : DomainModel
    {
        var key = $"{typeof(T)}-{id}";
        return await cache.GetOrCreateAsync(key, async entry =>
        {
            if (await databaseContext.GetById<T>(id) is not { } value) return null;
            entry.Value = value;
            entry.SlidingExpiration = TimeSpan.FromSeconds(10);
            return value;
        });
    }

    public async Task<T?> FirstOrDefault<T>(Expression<Func<T, bool>> predicate) where T : DomainModel
    {
        var hash = predicate.GetHashCode();
        return await cache.GetOrCreateAsync(hash, async entry =>
        {
            if (await databaseContext.FirstOrDefault(predicate) is not { } value) return null;
            entry.Value = value;
            entry.SlidingExpiration = TimeSpan.FromSeconds(10);
            return value;
        });
    }

    public async Task<TResult?> FirstOrDefaultTransformed<T, TResult>(
        Expression<Func<T, bool>> predicate, 
        Expression<Func<T, TResult>> transformFunction) where T : DomainModel
    {
        var hash = $"{predicate.GetHashCode()}-{transformFunction.GetHashCode()}";
        return await cache.GetOrCreateAsync(hash, async entry =>
        {
            if (await databaseContext.FirstOrDefaultTransformed(predicate, transformFunction) is not { } value) 
                return default;
            entry.Value = value;
            entry.SlidingExpiration = TimeSpan.FromSeconds(10);
            return value;
        });
    }
    
    // TODO: figure out how to best cache the results (is the predicate hash code going to be performant enough?),
    // TODO: do we have to split up the results in the list methods so each one can be accessed independently of the list?
    // TODO: seems like that would probably introduce a lot of additional code for not much benefit?

    public async Task<List<T>> List<T>(Expression<Func<T, bool>> predicate) where T : DomainModel
    {
        var hash = predicate.GetHashCode();
        return await cache.GetOrCreateAsync(hash, async entry =>
        {
            if (await databaseContext.ToList(predicate) is not { } value) return new();
            entry.Value = value;
            entry.SlidingExpiration = TimeSpan.FromSeconds(10);
            return value;
        }) ?? new();
    }

    public async Task<List<TResult>> ListTransformed<T, TResult>(
        Expression<Func<T, bool>> predicate, 
        Expression<Func<T, TResult>> transformFunction) where T : DomainModel
    {
        var hash = $"{predicate.GetHashCode()}-{transformFunction.GetHashCode()}";
        return await cache.GetOrCreateAsync(hash, async entry =>
        {
            if (await databaseContext.ToListTransformed(predicate, transformFunction) is not { } value)
                return new List<TResult>();
            entry.Value = value;
            entry.SlidingExpiration = TimeSpan.FromSeconds(10);
            return value;
        }) ?? new();
    }

    public async Task<bool> Any<T>(Expression<Func<T, bool>> predicate)
    {
        return await databaseContext.Any(predicate);
    }

    public async Task Add<T>(T entity) where T : DomainModel
    {
        if (await databaseContext.Add(entity))
        {
            await eventBus.PublishEntityEvent(entity, EventType.Add);
            foreach (var e in entity.Events)
                await eventBus.PublishEvent(e);
        }
    }

    public async Task Update<T>(T entity) where T : DomainModel
    {
        if (await databaseContext.Update(entity))
        {
            await eventBus.PublishEntityEvent(entity, EventType.Update);
            foreach (var e in entity.Events)
                await eventBus.PublishEvent(e);
        }
    }

    public async Task Delete<T>(T entity) where T : DomainModel
    {
        if (await databaseContext.Delete(entity))
        {
            await eventBus.PublishEntityEvent(entity, EventType.Delete);
            foreach (var e in entity.Events)
                await eventBus.PublishEvent(e);
        }
    }
}