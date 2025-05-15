using System.Text.Json.Serialization;

namespace ManagementSystem.Shared.Common.Response
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public bool Success { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Errors { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TraceId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, object>? Extensions { get; set; }


        public static ApiResponse<T> SuccessResponse(T data, string message = "Success", int statusCode = 200) =>
            new ApiResponse<T>
            {
                StatusCode = statusCode,
                Message = message,
                Success = true,
                Data = data
            };


        public static ApiResponse<T> FailureResponse(string message, int statusCode = 400, List<string>? errors = null, string? traceId = null) =>
            new ApiResponse<T>
            {
                StatusCode = statusCode,
                Message = message,
                Success = false,
                Errors = errors,
                TraceId = traceId
            };
    }
}