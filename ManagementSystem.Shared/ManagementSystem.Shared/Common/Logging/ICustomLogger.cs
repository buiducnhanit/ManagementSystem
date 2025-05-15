namespace ManagementSystem.Shared.Common.Logging
{
    public interface ICustomLogger<T>
    {
        void Info(string message, object? data = null, string? traceId = null);
        void Warn(string message, object? data = null, string? traceId = null);
        void Error(string message, Exception? ex = null, object? data = null, string? traceId = null);
        void Debug(string message, object? data = null, string? traceId = null);
    }
}
