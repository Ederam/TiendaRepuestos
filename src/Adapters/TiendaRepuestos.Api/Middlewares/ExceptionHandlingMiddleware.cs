namespace TiendaRepuestos.Api.Middlewares;

using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

/// <summary>
/// Middleware global para la captura y procesamiento unificado de excepciones no controladas en la API.
/// Implementa la especificación RFC 7807 (ProblemDetails).
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ExceptionHandlingMiddleware"/>.
    /// </summary>
    /// <param name="next">Delegado que representa el siguiente componente en el pipeline HTTP.</param>
    /// <param name="logger">Instancia de registro para capturar la traza del error.</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Intercepta la ejecución del flujo HTTP para capturar cualquier excepción no procesada.
    /// </summary>
    /// <param name="context">Encapsula toda la información de la petición y respuesta HTTP actual.</param>
    /// <returns>Una tarea asíncrona que representa la ejecución del middleware.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción no controlada capturada en el middleware: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Transforma la excepción capturada en un formato estandarizado RFC 7807 (ProblemDetails).
    /// </summary>
    /// <param name="context">El contexto HTTP para modificar la respuesta.</param>
    /// <param name="exception">La excepción capturada.</param>
    /// <returns>Una tarea que escribe la respuesta JSON en el flujo de salida.</returns>
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        HttpStatusCode statusCode;
        string title;
        string detailMessage = exception.Message;
        IDictionary<string, string[]>? validationErrors = null;

        switch (exception)
        {
            case ValidationException valEx:
                statusCode = HttpStatusCode.BadRequest;
                title = "Error de validación en los datos de entrada";
                validationErrors = valEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
                break;

            case DbUpdateConcurrencyException:
                statusCode = HttpStatusCode.Conflict;
                title = "Conflicto de concurrencia en inventario";
                detailMessage = "Los datos del repuesto fueron modificados simultáneamente por otra transacción. Por favor, consulte los datos actualizados y reintente la operación.";
                break;

            case InvalidOperationException:
                statusCode = HttpStatusCode.BadRequest;
                title = "Solicitud inválida";
                break;

            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                title = "Recurso no encontrado";
                break;
    
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                title = "Acceso no autorizado";
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;
                title = "Error interno del servidor";
                break;
        }

        context.Response.StatusCode = (int)statusCode;

        ProblemDetails problemDetails;

        if (validationErrors != null)
        {
            problemDetails = new HttpValidationProblemDetails(validationErrors)
            {
                Status = (int)statusCode,
                Title = title,
                Detail = "Consulte la propiedad 'errors' para conocer los detalles del fallo.",
                Instance = context.Request.Path
            };
        }
        else
        {
            problemDetails = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = detailMessage,
                Instance = context.Request.Path
            };
        }

        string jsonResponse = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return context.Response.WriteAsync(jsonResponse);
    }
}