namespace LMS.AdminPanel.Exceptions
{
    public class DeviceLimitExceededException : AppException
    {
        public DeviceLimitExceededException() : base("Maximum 2 devices allowed.", 400)
        {
        }
    }
}
