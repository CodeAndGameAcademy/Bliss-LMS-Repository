namespace LMS.Application.Exceptions
{
    public class ValidationException : AppException
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(Dictionary<string, string[]> errors) : base("Validation failed", 400)
        {
            Errors = errors;
        }
    }
}
