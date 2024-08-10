namespace Pneumatic.Clean.Domain.Users;

public interface IUserAccessor
{
    AppUser CurrentUser { get; }
}