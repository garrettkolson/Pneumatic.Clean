namespace Pneumatic.Clean.Domain.Users;

public record UserType(int Id, string Name)
{
    public static readonly UserType Admin = new(1, "Admin");
}