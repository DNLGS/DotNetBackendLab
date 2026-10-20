namespace DotNetFoundations.Middlewares
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggerMiddleware> _logger;
        public LoggerMiddleware(RequestDelegate next, ILogger<LoggerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                _logger.LogInformation("Request: {Host} {Method} {Path}. TraceId: {TraceId}",
                context.Request.Host.Value,
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);
                await _next(context);
                _logger.LogInformation("Response: {StatusCode}. TraceId: {TraceId}",
                    context.Response.StatusCode,
                    context.TraceIdentifier);
            }
            catch (Exception)
            {
                throw;
            }            
        }
    }
}
