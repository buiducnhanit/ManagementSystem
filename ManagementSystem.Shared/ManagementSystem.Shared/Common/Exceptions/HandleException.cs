namespace ManagementSystem.Shared.Common.Exceptions
{
    public class HandleException : Exception
    {
        public int StatusCode { get; }
        public List<string>? Errors { get; }


        public HandleException(string message, int statusCode, List<string>? errors = null) : base(message)
        {
            StatusCode = statusCode;
            Errors = errors;
        }
    }
}
