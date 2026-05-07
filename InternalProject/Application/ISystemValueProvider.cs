namespace InternalProject.Application;

public interface ISystemValueProvider
{
    Guid NewGuid();
    DateTime UtcNow { get; }
}
