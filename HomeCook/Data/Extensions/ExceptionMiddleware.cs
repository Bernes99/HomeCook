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

            switch (exception)
            {

                case RecipeException:
                    switch (exception.Message)
                    {
                        case RecipeException.ReicpeDoesntExist:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = exception.Message
                            }.ToString());
                            break;
                        case RecipeException.CommentDoesntExist:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = exception.Message
                            }.ToString());
                            break;
                        default:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = context.Response.StatusCode,
                                Message = exception.Message
                            }.ToString());
                            break;
                    }
                    break;

                case AuthException:
                    switch(exception.Message)
                    {
                        case AuthException.InvalidCode:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = exception.Message
                            }.ToString());
                            break;
                        case AuthException.TooManyLoginAttempts:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status423Locked,
                                Message = exception.Message
                            }.ToString());
                            break;
                        case AuthException.UserAlreadyExist:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = exception.Message
                            }.ToString());
                            break;
                        case AuthException.InvalidLoginAttempt:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = exception.Message
                            }.ToString());
                            break;
                        case AuthException.UserDoesNotExist:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = exception.Message
                            }.ToString());
                            break;
                        case AuthException.BadRequest:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = exception.Message
                            }.ToString());
                            break;
                        case AuthException.InvalidRefreshToken:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = exception.Message
                            }.ToString());
                            break;
                        default:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = context.Response.StatusCode,
                                Message = exception.Message
                            }.ToString());
                            break;
                    }
                    break;   
                    
                case CategoryException:
                    switch (exception.Message)
                    {
                        case CategoryException.CategoryAlreadyExist:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = exception.Message
                            }.ToString());
                            break;
                        case CategoryException.CategoryDoesntExist:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = exception.Message
                            }.ToString());
                            break;
                        case CategoryException.SomethingWentWrong:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status500InternalServerError,
                                Message = exception.Message
                            }.ToString());
                            break;
                        case CategoryException.CantAddManyOfTheSameCategories:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = exception.Message
                            }.ToString());
                            break;
                        default:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = context.Response.StatusCode,
                                Message = exception.Message
                            }.ToString());
                            break;

                    }
                    break;
                case ImageException:
                    switch (exception.Message)
                    {
                        case ImageException.ProfileImageError:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = exception.Message
                            }.ToString());
                            break;
                        case ImageException.UserhasNoProfileImage:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = exception.Message
                            }.ToString());
                            break;
                        case ImageException.RecipeImageError:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = exception.Message
                            }.ToString());
                            break;
                        case ImageException.RecipeHasNoMainImage:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = exception.Message
                            }.ToString());
                            break;
                        default:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = context.Response.StatusCode,
                                Message = exception.Message
                            }.ToString());
                            break;

                    }
                    break;
                case ProductException:
                    switch (exception.Message)
                    {
                        case ProductException.ProductCategoryAlreadyExist:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = exception.Message
                            }.ToString());
                            break;
                        case ProductException.ProductCategoryDoesntExist:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = exception.Message
                            }.ToString());
                            break;
                        case ProductException.SomethingWentWrong:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status500InternalServerError,
                                Message = exception.Message
                            }.ToString());
                            break;
                        case ProductException.ProductAlreadyExist:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = exception.Message
                            }.ToString());
                            break;
                        case ProductException.ProductDoesntExist:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = exception.Message
                            }.ToString());
                            break;
                        case ProductException.UserAlreadyHaveThisProduct:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = exception.Message
                            }.ToString());
                            break;
                        case ProductException.CantAddManyOfTheSameProducts:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = exception.Message
                            }.ToString());
                            break;
                        default:
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = context.Response.StatusCode,
                                Message = exception.Message
                            }.ToString());
                            break;

                    }
                    break;
                

                default:
                    await context.Response.WriteAsync(new ErrorDetails()
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = exception.Message
                    }.ToString());
                    break;
            }
            
        }
    }
}
