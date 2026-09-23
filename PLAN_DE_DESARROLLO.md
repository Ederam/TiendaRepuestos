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

### 🟡 Sprint 2: Casos de Uso Completos de Inventario y Categorías (EN PROCESO)
- [ ] **Caso de Uso:** Actualizar precio de venta y ajustar stock de un repuesto.
- [ ] **Caso de Uso:** Desactivar / Eliminar de forma lógica un repuesto.
- [ ] **Entidad Categoría:** Modelado en Dominio (`Categoria`), repositorio e integración con `Producto`.
- [ ] **Manejo Global de Excepciones:** Middleware o Filtro para transformar excepciones de dominio en respuestas HTTP limpias (`400 Bad Request`, `404 Not Found`, `500 Error`).
- [ ] **Consultas Avanzadas:** Paginación y filtros por categoría/estado.

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