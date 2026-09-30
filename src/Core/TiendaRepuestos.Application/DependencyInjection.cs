namespace TiendaRepuestos.Application;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

/// <summary>
/// Clase de extensión para el registro de servicios de la capa de aplicación en el contenedor IoC.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra los servicios del caso de uso y validadores en el contenedor de dependencias.
    /// </summary>
    /// <param name="services">Colección de servicios del contenedor IoC.</param>
    /// <returns>La colección de servicios actualizada.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registrar servicios de caso de uso
        services.AddScoped<Services.ProductoService>();
        services.AddScoped<Services.CategoriaService>();
        services.AddScoped<Services.VentaService>();

        // Registrar automáticamente todos los validadores de FluentValidation definidos en este ensamblado
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}