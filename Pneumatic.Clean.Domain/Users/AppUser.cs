namespace Pneumatic.Clean.Domain.Users;

public class AppUser
{
    public int Id { get; set; }
    public string? LoginEmail { get; set; }
    public string? Username { get; set; }
    public UserType UserType { get; set; } = UserType.Admin;
}