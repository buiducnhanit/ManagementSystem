using Serilog;

namespace ManagementSystem.Shared.Common.Logging
{
    public class SerilogCustomLogger<T> : ICustomLogger<T>
    {
        private readonly ILogger _logger;

        public SerilogCustomLogger()
        {
            _logger = Log.ForContext<T>();
        }

        public void Info(string message, object? data = null, string? traceId = null)
        {
            _logger.Information("[INFO] {Message} | Data: {@Data} | TraceId: {TraceId}", message, data, traceId);
        }

        public void Warn(string message, object? data = null, string? traceId = null)
        {
            _logger.Warning("[WARN] {Message} | Data: {@Data} | TraceId: {TraceId}", message, data, traceId);
        }

        public void Error(string message, Exception? ex = null, object? data = null, string? traceId = null)
        {
            _logger.Error(ex, "[ERROR] {Message} | Data: {@Data} | TraceId: {TraceId}", message, data, traceId);
        }

        public void Debug(string message, object? data = null, string? traceId = null)
        {
            _logger.Debug("[DEBUG] {Message} | Data: {@Data} | TraceId: {TraceId}", message, data, traceId);
        }
    }
}
