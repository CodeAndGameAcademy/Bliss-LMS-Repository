namespace LMS.AdminPanel.Exceptions
{
    public class AlreadyExistsException : AppException
    {
        public AlreadyExistsException(string message) : base(message, 409)
        {
        }
    }
}
