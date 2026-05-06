using InternalProject.Application;

namespace InternalProject.Infrastructure;

public class SystemValueProvider : ISystemValueProvider
{
    public Guid NewGuid()
    {
        return Guid.NewGuid();
    }

    public DateTime UtcNow => DateTime.UtcNow;
}
