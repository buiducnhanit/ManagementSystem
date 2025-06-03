using Serilog;
using Serilog.Events;

namespace ManagementSystem.Shared.Common.Logging
{
    public class SerilogCustomLogger<T> : ICustomLogger<T>
    {
        private readonly ILogger _logger;

        public SerilogCustomLogger()
        {
            _logger = Log.ForContext<T>();
        }

        public void Info(string messageTemplate, object? data = null, string? traceId = null, params object[] propertyValues)
        {
            LogWithContext(LogEventLevel.Information, messageTemplate, data, traceId, null, propertyValues);
        }

        public void Warn(string messageTemplate, object? data = null, string? traceId = null, params object[] propertyValues)
        {
            LogWithContext(LogEventLevel.Warning, messageTemplate, data, traceId, null, propertyValues);
        }

        public void Error(string messageTemplate, Exception? ex = null, object? data = null, string? traceId = null, params object[] propertyValues)
        {
            LogWithContext(LogEventLevel.Error, messageTemplate, data, traceId, ex, propertyValues);
        }

        public void Debug(string messageTemplate, object? data = null, string? traceId = null, params object[] propertyValues)
        {
            LogWithContext(LogEventLevel.Debug, messageTemplate, data, traceId, null, propertyValues);
        }

        private void LogWithContext(LogEventLevel level, string messageTemplate, object? data, string? traceId, Exception? ex, params object[] propertyValues)
        {
            var fullMessage = messageTemplate;
            var values = new List<object>(propertyValues);

            if (data != null)
            {
                fullMessage += " | Data: {@Data}";
                values.Add(data);
            }

            if (!string.IsNullOrEmpty(traceId))
            {
                fullMessage += " | TraceId: {TraceId}";
                values.Add(traceId);
            }

            if (ex != null)
                _logger.Write(level, ex, fullMessage, values.ToArray());
            else
                _logger.Write(level, fullMessage, values.ToArray());
        }
    }
}
