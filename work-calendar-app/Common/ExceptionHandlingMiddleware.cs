using HttpExceptions.Exceptions;
using System.Net;

namespace calendar_app.Common
{
    public record ExceptionResponse(HttpStatusCode StatusCode, string Description);
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }


        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            logger.LogError(ex, "An unexpected error occured.");

            var response = ex switch
            {
                BadRequestException e => new ExceptionResponse(HttpStatusCode.BadRequest, e.Message),
                NotFoundException e => new ExceptionResponse(HttpStatusCode.NotFound, e.Message),
                UnauthorizedException e => new ExceptionResponse(HttpStatusCode.Unauthorized, e.Message),
                ForbiddenException e => new ExceptionResponse(HttpStatusCode.Forbidden, e.Message),
                _ => new ExceptionResponse(HttpStatusCode.InternalServerError, "Internal server error. Please retry later.")
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)response.StatusCode;
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
