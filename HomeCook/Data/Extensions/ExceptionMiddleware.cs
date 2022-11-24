using HomeCook.Data.CustomException;
using HomeCook.Data.Extensions.Interfaces;
using System.Net;

namespace HomeCook.Data.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate Next;
        private readonly ILoggerManager Logger;
        public ExceptionMiddleware(RequestDelegate next, ILoggerManager logger)
        {
            Logger = logger;
            Next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await Next(httpContext);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            Logger.LogError($"Something went wrong: {exception.StackTrace}");

            switch (exception)
            {
                case AuthException:
                    await context.Response.WriteAsync(new ErrorDetails()
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Message = exception.Message//$"Exeption on: ${exception.Source} with message: {exception.Message}"
                    }.ToString());
                    break;

                default:
                    await context.Response.WriteAsync(new ErrorDetails()
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = exception.Message//$"Exeption on: ${exception.Source} with message: {exception.Message}"
                    }.ToString());
                    break;
            }
            
        }
    }
}
