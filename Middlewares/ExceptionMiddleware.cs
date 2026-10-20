
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetFoundations.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger)
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
                _logger.LogError(ex,
                    "Erro em {Method} {Path}. TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.TraceIdentifier);

                if (context.Response.HasStarted)
                    throw;

                var (status, title) = ex switch
                {
                    ArgumentException =>
                        (400, "Argumento inválido"),

                    ValidationException =>
                        (400, "Falha na validação"),

                    KeyNotFoundException =>
                        (404, "Recurso não encontrado"),

                    DbUpdateConcurrencyException =>
                        (409, "Conflito de concorrência"),

                    DbUpdateException =>
                        (500, "Falha ao salvar os dados"),

                    DbException =>
                        (500, "Erro no banco de dados"),

                    _ =>
                        (500, "Erro interno do servidor")
                };

                context.Response.Clear();

                context.Response.StatusCode = status;

                var problem = new ProblemDetails
                {
                    Status = status,
                    Title = title,
                    Detail = status >= 500
                        ? "Ocorreu um erro interno."
                        : ex.Message,
                    Instance = context.Request.Path
                };

                problem.Extensions["traceId"] =
                    context.TraceIdentifier;

                await context.Response.WriteAsJsonAsync(
                    problem,
                    options: null,
                    contentType: "application/problem+json");
            }
        }
    }
}
