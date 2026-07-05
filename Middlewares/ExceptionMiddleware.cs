using Serilog.Context;
using System.Text.Json;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.Exceptions;

namespace WebApplication1.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<ExceptionMiddleware> _Ilogger;
        public ExceptionMiddleware (RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _Ilogger = logger;
            _requestDelegate = next;
        }
        public async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            int statusCode = 500;
            string message = "A server error occurred.";
            switch (ex)
            {
                case NotFoundException notFoundEx:
                    statusCode = 404;
                    message = notFoundEx.Message;
                    break;

                case BadRequestException badRequestEx:
                    statusCode = 400;
                    message = badRequestEx.Message;
                    break;
                case UnauthorizedException unauthorizedEx:
                    statusCode = 403;
                    message = unauthorizedEx.Message;
                    break;
                case ConflictException conflictEx:
                    statusCode = 409;
                    message = conflictEx.Message;
                    break;
                case ValidationException validationEx:
                    statusCode = 400;
                    message = validationEx.Message;
                    break;
            }
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            var apiResponseDto = new ApiResponseDto<string>
            {
                StatusCode = statusCode,
                Data = null,
                IsSuccess = false,
                Message = message,
            };
           var json = JsonSerializer.Serialize(apiResponseDto);
            await context.Response.WriteAsync(json);
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _requestDelegate(context);
            }
            catch (Exception ex)
            {
                using (LogContext.PushProperty("RequestPath", context.Request.Path))
                {
                    if (ex is NotFoundException || ex is BadRequestException || ex is UnauthorizedException || ex is ConflictException)
                    {
                        _Ilogger.LogWarning(ex.Message);
                    }
                    else
                    {
                        _Ilogger.LogError(ex, "Unhandled Exception: {Message}", ex.Message);
                    }

                    await HandleExceptionAsync(context, ex);
                }
            }
        }
    }
}
