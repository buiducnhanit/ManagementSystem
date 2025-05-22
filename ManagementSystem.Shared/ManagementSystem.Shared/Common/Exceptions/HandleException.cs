namespace ManagementSystem.Shared.Common.Exceptions
{
    public class HandleException : Exception
    {
        public int StatusCode { get; }

        public HandleException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
