# 📝 Bitácora de Lecciones Aprendidas y Registros de Decisiones (ADR)

Este documento centraliza los hallazgos técnicos, decisiones arquitectónicas y resolución de problemas durante el desarrollo del proyecto **TiendaRepuestos**.

---

## 📌 Registros de Decisiones de Arquitectura (ADR)

### ADR-001: Selección de Arquitectura Hexagonal y Clean Architecture
* **Contexto:** Se requiere construir una API extensible y mantenible para la gestión de repuestos automotrices.
* **Decisión:** Organizar la solución en 4 capas desacopladas: `Domain`, `Application`, `Infrastructure` y `Api`.
* **Consecuencia:** Cero dependencias externas en la capa de Dominio, facilitando pruebas unitarias y garantizando la independencia tecnológica.

### ADR-002: Reemplazo de AutoMapper por Métodos Estáticos de Fábrica (`FromEntity`)
* **Contexto:** La transformación entre Entidades de Dominio y DTOs mediante AutoMapper utiliza *Reflection*, impactando el rendimiento y detectando errores solo en tiempo de ejecución.
* **Decisión:** Implementar el patrón `FromEntity` explícito directamente en los récords/DTOs de respuesta.
* **Consecuencia:** Máxima velocidad de ejecución en nanosegundos, prevención de errores en tiempo de compilación (*Type Safety*) y código 100% puro C#.

### ADR-003: Eliminación del uso de `var` por Tipado Explícito
* **Contexto:** Mantener un estándar estricto de legibilidad y claridad de código en todo el Backend.
* **Decisión:** Prohibir la declaración implícita `var` en favor del tipado explícito de variables (`IEnumerable<T>`, `WebApplicationBuilder`, `string?`, etc.).

### ADR-004: Inyección de Dependencias Modularizada por Capas (IoC)
* **Contexto:** Evitar que el archivo `Program.cs` de la API se convierta en un monolito inmanejable de registros de servicios y rompa el encapsulamiento al referenciar directamente implementaciones concretas de infraestructura.
* **Decisión:** Crear una clase estática `DependencyInjection.cs` con métodos de extensión de `IServiceCollection` (`AddApplication()`, `AddInfrastructure()`) dentro de cada biblioteca de clases.
* **Consecuencia:** Mantenibilidad, alto desacoplamiento y un `Program.cs` limpio con solo llamadas modulares de alto nivel.

---

## 🛠️ Registro de Errores y Lecciones Aprendidas

### 1. Error de Compilación `CS1520: Method must have a return type`
* **Síntoma:** Fallo de compilación en `ProductosController.cs` con el mensaje *Method must have a return type*.
* **Causa Raíz:** Inconsistencia en el nombre del constructor de la clase (`ParameterProductosController` en lugar de `ProductosController`). El compilador de C# interpretó la firma como un método ordinario sin tipo de retorno explícito.
* **Solución:** Renombrar el constructor para que coincida exactamente con el nombre de la clase `ProductosController`.
* **Lección Aprendida:** En C#, todo constructor debe coincidir punto por punto con el identificador de la clase; cualquier divergencia ortográfica hace que el compilador lo catalogue como un método regular roto.

---

### 2. Error de Conexión en pgAdmin: `connection timeout expired`
* **Síntoma:** pgAdmin no logró conectarse al servidor de PostgreSQL en `localhost:5433`.
* **Causa Raíz:** El contenedor de Docker que aloja la base de datos PostgreSQL se encontraba apagado o pausado.
* **Solución:** Ejecutar `docker compose up -d` desde la raíz de la solución para reactivar los servicios de infraestructura.
* **Lección Aprendida:** Verificar el estado activo de los contenedores (`docker ps`) antes de intentar consumir adaptadores de persistencia de datos.

---

### 3. Error FATAL de PostgreSQL: `database "repuestos_db" does not exist`
* **Síntoma:** Al intentar conectar pgAdmin a la base de datos mediante el puerto `5433`, PostgreSQL rechazaba la conexión indicando que la base `repuestos_db` no existía.
* **Causa Raíz:** El servidor de base de datos PostgreSQL dentro de Docker estaba corriendo, pero la base de datos lógica especificada aún no había sido inicializada en el catálogo.
* **Solución:** Conectarse inicialmente a la base por defecto `postgres` en pgAdmin para crear la base de datos `repuestos_db`, o aplicar las migraciones de Entity Framework Core mediante `dotnet ef database update`.
* **Lección Aprendida:** Separar la existencia de la instancia del servidor PostgreSQL de la creación específica del esquema y base de datos solicitada por la aplicación.

---

### 4. Error de Compilación en Class Library `CS0234 / CS0246`: `IServiceCollection` no encontrado
* **Síntoma:** Al modularizar la inyección de dependencias en `TiendaRepuestos.Application`, la compilación falla indicando que `IServiceCollection` o el espacio de nombres `Microsoft.Extensions.DependencyInjection` no existen.
* **Causa Raíz:** Los proyectos de tipo *Class Library* (`.csproj`) no incluyen por defecto el Framework Web de ASP.NET Core donde reside la abstracción de IoC.
* **Solución:** Agregar el paquete de abstracción ligero `Microsoft.Extensions.DependencyInjection.Abstractions` mediante la consola NuGet / CLI:
  `dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions`.
* **Lección Aprendida:** Para mantener Clean Architecture pura en bibliotecas de clases sin importar todo el framework pesado de Web API, se deben incluir únicamente las abstracciones ligeras de NuGet requeridas.

---

### 5. Error de Conexión en Migraciones EF Core: `NpgsqlException: Failed to connect to 127.0.0.1:5433`
* **Síntoma:** Al ejecutar `dotnet ef database update`, el CLI se detiene con error `System.Net.Sockets.SocketException (10061): No se puede establecer una conexión ya que el equipo de destino denegó expresamente dicha conexión`.
* **Causa Raíz:** El servicio o contenedor Docker de PostgreSQL no estaba corriendo en segundo plano al momento de desplegar la migración.
* **Solución:** Iniciar el servicio o contenedor de PostgreSQL (ej. `docker compose up -d` o `podman start`) antes de ejecutar el comando de actualización de base de datos.
* **Lección Aprendida:** El proceso de migración de EF Core valida físicamente el esquema ejecutando comandos SQL en vivo; la infraestructura debe estar operativa antes de correr comandos del CLI de `dotnet ef`.