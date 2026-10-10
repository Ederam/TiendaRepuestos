# 🗺️ Plan de Desarrollo: Sistema de Gestión de Repuestos (TiendaRepuestos)

**Arquitectura:** Hexagonal / Clean Architecture  
**Backend:** .NET 8, EF Core, PostgreSQL, DDD Lite  
**Frontend:** Angular 19  
**Estándar de Código:** C# con tipado explícito (sin `var`), contratos DTO segregados (In/Out) y métodos estáticos de fábrica (`FromEntity`).

---

## 📌 Estado de Sprints

### 🟢 Sprint 1: Fundamentos del Núcleo y MVP Vertical (COMPLETADO)

- [x] Modelado de entidad de dominio `Producto` y Value Objects.
- [x] Configuración de PostgreSQL en Podman/Docker (`docker-compose.yml`).
- [x] Creación de `ApplicationDbContext` y repositorios en la capa Infrastructure.
- [x] Implementación de DTOs de entrada (`CrearProductoDto`) y respuesta (`ProductoResponseDto`).
- [x] Creación del servicio de aplicación `ProductoService`.
- [x] Endpoints base expuestos mediante Swagger UI.

---

### 🟢 Sprint 2: Seguridad, Validaciones, Reglas de Venta y RBAC (COMPLETADO)

- [x] Validación declarativa con FluentValidation para DTOs.
- [x] Entidad `Categoria` y relación de catálogo.
- [x] Agregado de ventas, cálculo fiscal y descuento transaccional de stock.
- [x] Middleware global de excepciones RFC 7807 (`ProblemDetails`).
- [x] Módulo de identidad, hashing seguro de contraseñas con BCrypt y tokens JWT.
- [x] Integración de Swagger UI con soporte Bearer Token.
- [x] Control de Acceso Basado en Roles (RBAC) con constantes fuertemente tipadas (`Roles.cs`).
- [x] Cobertura de pruebas unitarias con xUnit, Moq y FluentAssertions (`VentaServiceTests`, `AuthServiceTests`).

---

### 🟢 Sprint 3: Reportes Analíticos, Concurrencia y Paginación (COMPLETADO)

- [x] Consultas analíticas de alto rendimiento con Dapper e inyección de `IReportesRepository`.
- [x] Endpoints de reporte para Stock Crítico y Ranking de Top Repuestos Más Vendidos.
- [x] Control de concurrencia optimista en inventario con token de sistema `xmin` de PostgreSQL.
- [x] Captura y traducción de `DbUpdateConcurrencyException` a código HTTP `409 Conflict` (RFC 7807).
- [x] Paginación formal y filtros avanzados en el catálogo de productos (`PagedResult<T>`).

---

### 🔵 Sprint 4: Frontend Web SPA (Angular 19)

- [ ] Inicialización del proyecto Angular 19 (Standalone Components, Signals).
- [ ] Configuración de cliente HTTP (`HttpClient`) e interceptor funcional para Bearer Token JWT.
- [ ] Configuración de CORS en la API de .NET 8.
- [ ] Módulos/Vistas: Catálogo paginado con filtros, punto de venta (POS) y panel de reportes analíticos.

---

### 🟠 Sprint 5: DevOps, Contenerización Total y Despliegue

- [ ] Empaquetado en contenedor del Backend (.NET 8) mediante `Dockerfile` multi-stage.
- [ ] Empaquetado del Frontend (Angular 19) en Nginx.
- [ ] Archivo `docker-compose.yml` de producción unificado (API + PostgreSQL + Angular SPA).
