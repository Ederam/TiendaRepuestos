# 🗺️ Plan de Desarrollo: Sistema de Gestión de Repuestos (TiendaRepuestos)

**Arquitectura:** Hexagonal / Clean Architecture  
**Backend:** .NET 8, EF Core, PostgreSQL, DDD Lite  
**Frontend:** Angular 19  
**Estándar de Código:** C# con tipado explícito (sin `var`), contratos DTO segregados (In/Out) y métodos estáticos de fábrica (`FromEntity`).

---

## 📌 Estado de Sprints

### 🟢 Sprint 1: Fundamentos del Núcleo y MVP Vertical (COMPLETADO)

- [x] Modelado de entidad de dominio `Producto` y Value Objects.
- [x] Configuración de PostgreSQL en Docker (`docker-compose.yml`).
- [x] Creación de `ApplicationDbContext` y repositorios en la capa Infrastructure.
- [x] Implementación de DTOs de entrada (`CrearProductoDto`) y respuesta (`ProductoResponseDto`).
- [x] Creación del servicio de aplicación `ProductoService`.
- [x] Endpoint `POST` y `GET` en `ProductosController` expuestos mediante Swagger UI.

---

### 🟢 Sprint 2: Seguridad, Validaciones, Reglas de Venta y RBAC (COMPLETADO)

- [x] Validación declarativa con FluentValidation para todos los DTOs de entrada.
- [x] Agregado de ventas, cálculo de impuestos, totales y ajuste de stock transaccional.
- [x] Cobertura de pruebas unitarias para `VentaService` (stock insuficiente, comprobante inválido, producto inexistente).
- [x] Módulo de identidad y hashing seguro de contraseñas con BCrypt.
- [x] Generación de tokens JWT y autenticación Bearer en pipeline HTTP.
- [x] Integración de OpenAPI/Swagger con candado de autorización Bearer.
- [x] Control de Acceso Basado en Roles (RBAC) desacoplado en controladores mediante `Roles.cs`.
- [x] Cobertura de pruebas unitarias para `AuthService` (login exitoso, credenciales erróneas, usuario inactivo, emails duplicados).

---

### 🔵 Sprint 3: Arquitectura y Consumo desde Frontend (Angular 19)

- [ ] Inicialización del proyecto Angular 19.
- [ ] Configuración de cliente HTTP (`HttpClient`) e interfaces de TypeScript mapeadas a los DTOs de C#.
- [ ] Configuración de CORS en la API de .NET 8 para permitir peticiones desde el frontend.
- [ ] Creación de módulos/componentes: Catálogo de repuestos, formulario de creación y ajuste de stock.

---

### 🟣 Sprint 4: Pruebas Unitarias y Blindaje de Código

- [ ] Pruebas unitarias para la entidad de dominio `Producto` (reglas de negocio).
- [ ] Pruebas unitarias para `ProductoService` usando `Moq` / `NSubstitute`.
- [ ] Pruebas de integración para los controladores de la API.

---

### 🟠 Sprint 5: Observabilidad, DevOps y Despliegue

- [ ] Configuración de registro de eventos (Logging/Telemetry).
- [ ] Empaquetado en Docker del Backend (.NET 8) mediante `Dockerfile`.
- [ ] Configuración final de docker-compose unificado (Backend + Database + Frontend).
