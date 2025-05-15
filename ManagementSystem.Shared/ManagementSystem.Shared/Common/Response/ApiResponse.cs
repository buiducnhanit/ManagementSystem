namespace ManagementSystem.Shared.Common.Response
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public bool Success { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public string? TraceId { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success", int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                Message = message,
                Success = true,
                Data = data
            };
        }

        public static ApiResponse<T> FailureResponse(string message, int statusCode = 400, List<string>? errors = null, string? traceId = null)
        {
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                Message = message,
                Success = false,
                Errors = errors,
                TraceId = traceId
            };
        }
    }
}