namespace TiendaRepuestos.Application;

using Microsoft.Extensions.DependencyInjection;
using TiendaRepuestos.Application.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ProductoService>();
        services.AddScoped<CategoriaService>();
        services.AddScoped<VentaService>();

        return services;
    }
}