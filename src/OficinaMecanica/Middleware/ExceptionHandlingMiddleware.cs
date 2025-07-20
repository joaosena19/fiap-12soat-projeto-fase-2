using System.Net;
using System.Text.Json;
using Shared.Exceptions;

namespace API.Middleware
{
    /// <summary>
    /// Middleware para tratamento centralizado de exceções
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                message = string.Empty,
                statusCode = 0
            };

            switch (exception)
            {
                case DomainException domainEx:
                    _logger.LogWarning(domainEx, "Domain exception occurred: {Message}", domainEx.Message);
                    response = new
                    {
                        message = domainEx.Message,
                        statusCode = (int)domainEx.StatusCode
                    };
                    context.Response.StatusCode = (int)domainEx.StatusCode;
                    break;

                default:
                    _logger.LogError(exception, "An unexpected error occurred: {Message}", exception.Message);
                    response = new
                    {
                        message = "Ocorreu um erro interno no servidor.",
                        statusCode = (int)HttpStatusCode.InternalServerError
                    };
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
