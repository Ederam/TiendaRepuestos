using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TiendaRepuestos.Api.Middlewares;
using TiendaRepuestos.Application;
using TiendaRepuestos.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// 1. Registro Modularizado de Dependencias por Capas
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Configuración de Autenticación con JWT Bearer
string jwtSecret = builder.Configuration["JwtSettings:Secret"]
    ?? throw new InvalidOperationException("Falta configurar JwtSettings:Secret");
string jwtIssuer = builder.Configuration["JwtSettings:Issuer"]
    ?? throw new InvalidOperationException("Falta configurar JwtSettings:Issuer");
string jwtAudience = builder.Configuration["JwtSettings:Audience"]
    ?? throw new InvalidOperationException("Falta configurar JwtSettings:Audience");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ClockSkew = TimeSpan.Zero // Elimina la tolerancia estándar de 5 minutos al expirar
    };
});

builder.Services.AddAuthorization();

// 3. Controllers y Documentación Swagger con Soporte para Tokens Bearer
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Tienda Repuestos API",
        Version = "v1",
        Description = "API RESTful con Clean Architecture para POS e Inventario de Repuestos"
    });

    // Definición de esquema de seguridad Bearer
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT obtenido del endpoint de login (ejemplo: Bearer {token})"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

WebApplication app = builder.Build();

// 4. Pipeline HTTP: Middleware Global de Excepciones
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 5. Autenticación y Autorización (El orden es mandatorio)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();