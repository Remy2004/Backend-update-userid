using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Aoun.API.Middleware;
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next; private readonly ILogger<ExceptionMiddleware> _logger;
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger) { _next = next; _logger = logger; }
    public async Task InvokeAsync(HttpContext context) {
        try { await _next(context); }
        catch (Exception ex) {
            _logger.LogError(ex, "❌ حدث خطأ غير متوقع");
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { StatusCode = context.Response.StatusCode, Message = "حدث خطأ داخلي", DetailedError = ex.InnerException != null ? ex.InnerException.Message : ex.Message }));
        }
    }
}


