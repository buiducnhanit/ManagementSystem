﻿using Serilog;

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
            _logger.Information("{Message} | Data: {@Data} | TraceId: {TraceId}", message, data, traceId);
        }

        public void Warn(string message, object? data = null, string? traceId = null)
        {
            _logger.Warning("{Message} | Data: {@Data} | TraceId: {TraceId}", message, data, traceId);
        }

        public void Error(string message, Exception? ex = null, object? data = null, string? traceId = null)
        {
            _logger.Error(ex, "{Message} | Data: {@Data} | TraceId: {TraceId}", message, data, traceId);
        }

        public void Debug(string message, object? data = null, string? traceId = null)
        {
            _logger.Debug("{Message} | Data: {@Data} | TraceId: {TraceId}", message, data, traceId);
        }
    }
}
