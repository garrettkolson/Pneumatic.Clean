namespace Pneumatic.Domain;

public record Result<T>(T? Ok, Exception? Error)
{
    public bool IsOk => Ok != null;
    public bool IsError => Error != null;
}