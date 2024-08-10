using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Memory;
using Pneumatic.Clean.Domain.Changes;
using Pneumatic.Clean.Domain.Events;

namespace Pneumatic.Clean.Domain.Repositories;

// TODO: eventually configure this to use a RepoConfig (with cache settings, etc)

// TODO: if we decide that using domain models in the db is not acceptable,
// TODO: configure a dictionary to map domain types to db types...
// TODO: then, a dictionary to map that tuple to a transform func

public abstract class RepositoryBase<TModelBase>(
        EntityUpdateEventFactory factory,
        IEventBusManager eventBus,
        IMemoryCache cache,
        IDatabaseContext databaseContext)
    : IReadRepository<TModelBase>, IWriteRepository<TModelBase>
    where TModelBase : DomainModel
{
    public async Task<T?> GetById<T>(int id) where T : TModelBase
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

    public async Task<T?> FirstOrDefault<T>(Expression<Func<T, bool>> predicate) where T : TModelBase
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
        Expression<Func<T, TResult>> transformFunction) where T : TModelBase
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

    public async Task<List<T>> List<T>(Expression<Func<T, bool>> predicate) where T : TModelBase
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
        Expression<Func<T, TResult>> transformFunction) where T : TModelBase
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

    public async Task Add<T>(T entity) where T : TModelBase
    {
        if (await databaseContext.Add(entity) is not { } changes) return;
        await PublishAllEvents(entity, changes, EventType.Add);
    }

    public async Task Update<T>(T entity) where T : TModelBase
    {
        if (await databaseContext.Update(entity) is not { } changes) return;
        await PublishAllEvents(entity, changes, EventType.Update);
    }

    public async Task Delete<T>(T entity) where T : TModelBase
    {
        if (await databaseContext.Delete(entity) is not { } changes) return;
        await PublishAllEvents(entity, changes, EventType.Delete);
    }

    private async Task PublishAllEvents<T>(T entity, List<ChangedField> changes, EventType type) where T : TModelBase
    {
        await Task.WhenAll(entity.Events.Select(eventBus.PublishDomainEvent));
        await GetAndPublishEntityEvent(entity, changes, type);   
    }

    private async Task GetAndPublishEntityEvent<T>(T entity, List<ChangedField> changes, EventType type)
        where T : TModelBase
    {
        // TODO: do something with the returned error
        if (factory.GetEntityUpdateEvent(entity, changes) is not { IsOk: true } result) return;
        await eventBus.PublishEntityEvent(result.Value, type);
    }
}