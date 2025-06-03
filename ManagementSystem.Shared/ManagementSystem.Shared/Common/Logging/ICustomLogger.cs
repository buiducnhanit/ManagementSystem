namespace ManagementSystem.Shared.Common.Logging
{
    public interface ICustomLogger<T>
    {
        void Info(string messageTemplate, object? data = null, string? traceId = null, params object[] propertyValues);
        void Warn(string messageTemplate, object? data = null, string? traceId = null, params object[] propertyValues);
        void Error(string messageTemplate, Exception? ex = null, object? data = null, string? traceId = null, params object[] propertyValues);
        void Debug(string messageTemplate, object? data = null, string? traceId = null, params object[] propertyValues);
    }
}
