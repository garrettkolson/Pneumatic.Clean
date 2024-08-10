namespace Pneumatic.Clean.Domain.Configuration;

public interface IConfigurationAccessor
{
    string Environment { get; }
    string ModuleName { get; }
}