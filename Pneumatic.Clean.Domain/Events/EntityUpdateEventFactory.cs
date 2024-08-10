using Pneumatic.Clean.Domain.Changes;
using Pneumatic.Clean.Domain.Configuration;
using Pneumatic.Clean.Domain.Exceptions;
using Pneumatic.Clean.Domain.Users;

namespace Pneumatic.Clean.Domain.Events;

public class EntityUpdateEventFactory(
    IUserAccessor userAccessor,
    IConfigurationAccessor configAccessor,
    IEntityUpdateStackAccessor stackAccessor)
{
    public Result<EntityUpdateEvent, EntityUpdateException> GetEntityUpdateEvent<T>(T entity, List<ChangedField> changes)
        where T : DomainModel
    {
        if (stackAccessor.HasExceededMaxDepth)
            return Result<EntityUpdateEvent, EntityUpdateException>.Err(new EntityUpdateException());

        return Result<EntityUpdateEvent, EntityUpdateException>.Ok(new EntityUpdateEvent
        {
            Changes = changes,
            Entity = entity,
            EntityType = entity.GetType(),
            KeyId = entity.KeyId,
            Metadata = new(DateTime.UtcNow, configAccessor.ModuleName),
            OriginatingUser = stackAccessor.OriginatingUser,
            PreviousEvent = stackAccessor.CurrentEvent,
            User = userAccessor.CurrentUser
        });
    }
}