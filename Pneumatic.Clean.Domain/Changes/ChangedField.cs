namespace Pneumatic.Clean.Domain.Changes;

public record ChangedField(string Name, object OldValue, object NewValue);