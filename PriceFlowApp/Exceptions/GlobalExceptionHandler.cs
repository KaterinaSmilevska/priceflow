using Microsoft.AspNetCore.Diagnostics;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Exceptions
{
    public class GlobalExceptionHandler: IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An unhandled exception occured.");

            int statusCode;
            string code;
            string message;

            switch(exception)
            {
                case ValidationException ex:
                    statusCode = StatusCodes.Status400BadRequest;
                    code = ex.Code;
                    message = ex.Message;
                    break;
                case BusinessRuleException ex:
                    statusCode = StatusCodes.Status400BadRequest;
                    code = ex.Code;
                    message = ex.Message;
                    break;
                case AlreadyExistsException ex:
                    statusCode = StatusCodes.Status409Conflict;
                    code = ex.Code;
                    message = ex.Message;
                    break;
                case NotFoundException ex:
                    statusCode = StatusCodes.Status404NotFound;
                    code = ex.Code;
                    message = ex.Message;
                    break;
                case UnauthorizedException ex:
                    statusCode = StatusCodes.Status401Unauthorized;
                    code = ex.Code;
                    message = ex.Message;
                    break;
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    code = "INTERNAL_SERVER_ERROR";
                    message = "An unexpected error occured.";
                    break;
            }
            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(
                new ErrorResponse(code, message),
                cancellationToken
                );
            
            return true;
        }
    }
}
