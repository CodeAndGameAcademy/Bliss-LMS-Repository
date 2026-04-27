using LMS.StudentAPI.Exceptions;
using System.Net;
using System.Text.Json;

namespace LMS.StudentAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode status = HttpStatusCode.InternalServerError;

            switch (ex)
            {
                case ValidationException:
                case ArgumentException:
                case BadRequestException:
                    status = HttpStatusCode.BadRequest;
                    break;

                case UnauthorizedException:
                    status = HttpStatusCode.Unauthorized;
                    break;

                case ForbiddenException:
                    status = HttpStatusCode.Forbidden;
                    break;

                case NotFoundException:
                    status = HttpStatusCode.NotFound;
                    break;

                case AlreadyExistsException:
                case ConflictException:
                    status = HttpStatusCode.Conflict;
                    break;


                case DeviceLimitExceededException:
                    status = HttpStatusCode.TooManyRequests;
                    break;

                default:
                    status = HttpStatusCode.InternalServerError;
                    break;
            }

            var response = new
            {
                status = (int)status,
                message = ex.Message
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
