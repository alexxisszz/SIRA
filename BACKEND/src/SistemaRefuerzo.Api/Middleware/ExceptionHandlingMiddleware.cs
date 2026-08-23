using System.Net;
using SistemaRefuerzo.Application.Common.Exceptions;

namespace SistemaRefuerzo.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            await EscribirRespuestaAsync(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (CredencialesInvalidasException ex)
        {
            await EscribirRespuestaAsync(context, HttpStatusCode.Unauthorized, ex.Message);
        }
        catch (ReglaDeNegocioException ex)
        {
            await EscribirRespuestaAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error no controlado procesando {Metodo} {Ruta}", context.Request.Method, context.Request.Path);
            await EscribirRespuestaAsync(context, HttpStatusCode.InternalServerError, "Ocurrió un error inesperado.");
        }
    }

    private static async Task EscribirRespuestaAsync(HttpContext context, HttpStatusCode statusCode, string mensaje)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsJsonAsync(new { mensaje });
    }
}