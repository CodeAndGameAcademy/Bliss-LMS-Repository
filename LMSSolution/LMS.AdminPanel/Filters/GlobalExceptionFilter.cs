using LMS.AdminPanel.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LMS.AdminPanel.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var message = context.Exception.Message;

            context.HttpContext.Items["ErrorMessage"] = message;

            context.Result = new ViewResult
            {
                ViewName = context.ActionDescriptor.RouteValues["action"]
            };

            context.ExceptionHandled = true;
        }
    }
}
