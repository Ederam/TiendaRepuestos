namespace TiendaRepuestos.Application;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

/// <summary>
/// Clase de extensi�n para el registro de servicios de la capa de aplicaci�n en el contenedor IoC.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra los servicios del caso de uso y validadores en el contenedor de dependencias.
    /// </summary>
    /// <param name="services">Colecci�n de servicios del contenedor IoC.</param>
    /// <returns>La colecci�n de servicios actualizada.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registrar servicios de caso de uso
        services.AddScoped<Services.ProductoService>();
        services.AddScoped<Services.CategoriaService>();
        services.AddScoped<Services.VentaService>();
        services.AddScoped<Services.AuthService>();

        // Registrar autom�ticamente todos los validadores de FluentValidation definidos en este ensamblado
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}