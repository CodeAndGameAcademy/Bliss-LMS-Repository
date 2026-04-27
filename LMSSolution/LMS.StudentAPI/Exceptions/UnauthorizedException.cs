namespace LMS.StudentAPI.Exceptions
{
    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message = "Unauthorized access") : base(message, 401)
        {
        }
    }
}
