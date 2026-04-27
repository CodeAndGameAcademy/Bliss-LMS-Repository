namespace LMS.StudentAPI.Exceptions
{
    public class DeviceLimitExceededException : AppException
    {
        public DeviceLimitExceededException() : base("Maximum 2 devices allowed.", 400)
        {
        }
    }
}
