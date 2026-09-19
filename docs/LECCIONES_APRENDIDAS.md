# 📓 Bitácora de Desarrollo y Lecciones Aprendidas

## Proyecto: Sistema POS & Inventario de Repuestos
* **Stack Principal:** .NET 8 (Web API) + Angular 19 + PostgreSQL + Azure DevOps.
* **Arquitectura:** Hexagonal / Clean Architecture.

---

### 📑 Registro de Decisiones de Arquitectura (ADR)

#### ADR-001: Estructura de Capas e Inversión de Dependencias
* **Fecha:** Septiembre 2026
* **Contexto:** Se requiere un sistema desacoplado, mantenible a largo plazo y preparado para evolucionar a modelo SaaS.
* **Decisión:** Implementar Arquitectura Hexagonal separando la lógica en `Domain`, `Application`, `Infrastructure` y `Api`.
* **Consecuencia:** 
  * El proyecto `Domain` no posee referencias a NuGet externos ni a bases de datos.
  * La infraestructura (PostgreSQL, Entity Framework Core) depende de las interfaces definidas en la capa de aplicación/dominio.

---

### 💡 Lecciones Aprendidas & Buenas Prácticas

1. **Uso de `.gitignore` desde la inicialización:**
   * Evita subir carpetas `bin/`, `obj/`, y configuraciones locales como `appsettings.Development.json` con claves reales.
2. **Encapsulamiento del Dominio:**
   * Las entidades utilizan constructores privados o con validación explícita para evitar que los datos ingresen en estados inválidos.

#### Troubleshooting: Error NU1301 - Espacio insuficiente en Disco C
* **Problema:** Fallo en `dotnet build` por falta de almacenamiento en `C:\Users\CAMILO\AppData\Local\Temp\NuGetScratch`.
* **Causa:** La caché de NuGet y temporales del SDK de .NET operan en el disco del sistema (`C:`), aunque el proyecto esté alojado en `D:`.
* **Solución:** Ejecutar `dotnet nuget locals all --clear` y liberar espacio en la unidad de sistema.

#### ADR-002: Encapsulamiento del Dominio y Puertos de Salida
* **Decisión:** La entidad `Producto` utiliza setters privados y métodos explícitos (`ReducirStock`, `AumentarStock`) para evitar estados inconsistentes en la base de datos.
* **Beneficio:** Se garantiza que ninguna regla de negocio o control de inventario se salte desde la capa de API o de Infraestructura.

#### Troubleshooting: Conflicto de Puerto 5432 con PostgreSQL Nativo de Windows
* **Problema:** Error `28P01` por colisión de servicios escuchando en el puerto local `5432`.
* **Causa:** Un servicio de PostgreSQL instalado localmente en Windows estaba interceptando las peticiones enviadas al contenedor.
* **Solución:** Mapear el contenedor al puerto externo `5433` (`5433:5432`) en `docker-compose.yml` y actualizar la cadena de conexión en `appsettings.json`.

