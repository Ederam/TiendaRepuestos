# 🏛️ Arquitectura Limpia (Clean Architecture) y Arquitectura Hexagonal en .NET 8

Este documento detalla los principios conceptuales, técnicos y la estructura práctica de la arquitectura utilizada en el proyecto **TiendaRepuestos**.

---

## 🎯 1. ¿Qué es la Arquitectura Limpia y Hexagonal?

La **Arquitectura Limpia** (introducida por Robert C. Martin / *Uncle Bob*) y la **Arquitectura Hexagonal** (o *Puertos y Adaptadores*, propuesta por Alistair Cockburn) son patrones de diseño de software enfocados en la **separación de responsabilidades** y el **desacoplamiento tecnológico**.

### Principle Fundamental: La Regla de Dependencia
Las dependencias del código **siempre deben apuntar hacia adentro**. 
* Las capas externas (pantallas, bases de datos, APIs, servicios de terceros) dependen de las capas internas.
* El núcleo del sistema (**Dominio**) no sabe nada sobre la base de datos, los frameworks web o la interfaz de usuario.

```
       [ Adapters: Web API / UI ]
                   │
                   ▼
       [ Infrastructure: EF Core / PostgreSQL ]
                   │
                   ▼
       [ Core: Application (Casos de Uso) ]
                   │
                   ▼
       [ Core: Domain (Reglas de Negocio / Entidades) ]  <-- NÚCLEO PURO
```

---

## 💡 2. ¿Por qué se utiliza y cuáles son sus ventajas?

1. **Independencia de Frameworks y Herramientas:** 
   El motor del negocio no depende de .NET Core, Entity Framework, PostgreSQL ni Angular. Si en el futuro cambiamos la base de datos de PostgreSQL a MongoDB, **las reglas del negocio y casos de uso no cambian**.

2. **Facilidad de Pruebas Unitarias (Testability):**
   Las reglas de negocio se pueden probar de forma aislada sin necesidad de levantar una base de datos real o un servidor web.

3. **Inmunidad al Cambio Tecnológico:**
   Las decisiones de infraestructura (como librerías de ORM, clientes HTTP, proveedores de correo) se postergan o pueden ser reemplazadas sin reescribir el núcleo del software.

4. **Reglas de Negocio Protegidas:**
   Las validaciones críticas (como impedir stocks negativos o precios en cero) viven agrupadas en las entidades de dominio y no dispersas en controladores HTTP.

---

## 🧱 3. Estructura de Capas en `TiendaRepuestos`

El proyecto está dividido en cuatro proyectos principales dentro de la solución `.sln`:

```
TiendaRepuestos/
├── src/
│   ├── Core/
│   │   ├── TiendaRepuestos.Domain/         (Capa 1: Dominio)
│   │   └── TiendaRepuestos.Application/    (Capa 2: Aplicación)
│   ├── Infrastructure/
│   │   └── TiendaRepuestos.Infrastructure/ (Capa 3: Infraestructura)
│   └── Adapters/
│       └── TiendaRepuestos.Api/            (Capa 4: Adaptador API)
└── tests/
    └── TiendaRepuestos.UnitTests/          (Pruebas Unitarias)
```

---

### 🟢 Capa 1: Dominio (`TiendaRepuestos.Domain`)
* **Qué contiene:** Entidades del negocio (`Producto`, `Categoria`), enumeradores (`EstadoProducto`), reglas de validación defensiva y **Interfaces de Puertos** (`IProductoRepository`).
* **Dependencias:** **Cero dependencias externas.** C# puro (.NET Standard / Core biblioteca).
* **Concepto clave:** Define el *qué* necesita el negocio, no el *cómo* se implementa.

---

### 🟡 Capa 2: Aplicación (`TiendaRepuestos.Application`)
* **Qué contiene:** Casos de uso (`ProductoService`), objetos de transferencia de datos (`CrearProductoDto`, `ProductoResponseDto`) y contratos.
* **Dependencias:** Únicamente depende de `TiendaRepuestos.Domain`.
* **Concepto clave:** Coordina el flujo de datos entre las entidades de dominio y los adaptadores de entrada/salida.

---

### 🔵 Capa 3: Infraestructura (`TiendaRepuestos.Infrastructure`)
* **Qué contiene:** Implementaciones concretas de la persistencia de datos (`ApplicationDbContext`, `ProductoRepository`), configuraciones de Entity Framework Core (Fluent API) y migraciones.
* **Dependencias:** Depende de `TiendaRepuestos.Domain` (implementa sus puertos) y paquetes NuGet de PostgreSQL/EF Core.
* **Concepto clave:** Adaptador secundario que resuelve la comunicación con la base de datos.

---

### 🔴 Capa 4: Adaptadores API (`TiendaRepuestos.Api`)
* **Qué contiene:** Controladores HTTP (`ProductosController`), configuración del contenedor de Inyección de Dependencias (`Program.cs`) y Swagger UI.
* **Dependencias:** Depende de `Application` e `Infrastructure` para realizar la composición del inicio de la aplicación.
* **Concepto clave:** Adaptador primario de entrada que expone el sistema al exterior mediante HTTP/REST.

---

## 🔄 4. Flujo de Ejecución Horizontal (Petición HTTP)

Cuando un cliente ejecuta un `POST /api/productos`:

1. **API (`ProductosController`):** Recibe el DTO `CrearProductoDto` vía HTTP.
2. **Aplicación (`ProductoService`):** Procesa el caso de uso, invoca el constructor de la entidad `Producto`.
3. **Dominio (`Producto`):** Ejecuta sus validaciones internas (precio > 0, stock >= 0).
4. **Aplicación (`ProductoService`):** Llama a `IProductoRepository.AddAsync()`.
5. **Infraestructura (`ProductoRepository`):** Traduce la llamada a SQL mediante Entity Framework Core y persiste los cambios en la base de datos PostgreSQL dentro de Docker.
6. **Aplicación/API:** Devuelve `ProductoResponseDto.FromEntity()` transformado mediante mapeo explícito al cliente.