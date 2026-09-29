using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using TiendaRepuestos.Api.Middlewares;
using TiendaRepuestos.Application;
using TiendaRepuestos.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// 1. Registro Modularizado de Dependencias por Capas (Clean Architecture)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Configuración de API, Controllers y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// 3. Registrar Middleware Global de Excepciones
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();