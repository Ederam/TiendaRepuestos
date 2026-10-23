namespace TiendaRepuestos.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TiendaRepuestos.Application.Ports;
using TiendaRepuestos.Domain.Ports;
using TiendaRepuestos.Infrastructure.Authentication;
using TiendaRepuestos.Infrastructure.Persistence;
using TiendaRepuestos.Infrastructure.Persistence.Repositories;
using TiendaRepuestos.Infrastructure.Services;

/// <summary>
/// Módulo de configuración para registrar y resolver dependencias de la capa de Infraestructura.
/// Asocia los contratos de repositorio y servicios atómicos con sus implementaciones de PostgreSQL.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Configura el DbContext con Npgsql y registra los repositorios y servicios de infraestructura en el contenedor IoC.
    /// </summary>
    /// <param name="services">Contenedor de servicios de la aplicación.</param>
    /// <param name="configuration">Instancia de configuración para leer la cadena de conexión.</param>
    /// <returns>La misma colección de servicios para encadenamiento fluido.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IVentaRepository, VentaRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IJwtGenerator, JwtGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IReportesRepository, ReportesRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();

        // Servicio generador atómico de secuencias para comprobantes de venta
        services.AddScoped<IConsecutivoService, ConsecutivoService>();

        return services;
    }
}