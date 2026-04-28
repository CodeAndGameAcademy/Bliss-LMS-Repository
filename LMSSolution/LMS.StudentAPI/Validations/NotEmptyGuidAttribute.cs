using System.ComponentModel.DataAnnotations;

namespace LMS.StudentAPI.Validations
{
    public class NotEmptyGuidAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is Guid guid)
            {
                return guid != Guid.Empty;
            }
            return false;
        }
    }
}
