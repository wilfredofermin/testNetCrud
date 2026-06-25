# Explicación completa del proyecto testNet

## Arquitectura general

API REST en .NET 8 con **Domain-Driven Design** en 4 capas estrictas con dependencia unidireccional:

```
API → Infrastructure → Application → Domain
```

El flujo de una petición típica (ej. crear producto `POST /api/products`) es:

```
HTTP Request
  → ExceptionMiddleware (captura errores)
    → ProductsController.Create()
      → IProductService.CreateAsync()        [Application]
        → CreateProductValidator.Validate()  [Application - validación]
          → ProductDomainService.CreateProductAsync() [Domain - reglas de negocio]
            → IProductRepository.ExistsByCodeAsync()  [Interface en Domain]
              → ProductRepository (EF InMemory)       [Infrastructure]
            → new Product()                           [Domain - entidad]
          → _repository.AddAsync()           [Infrastructure]
        → Product.ToDto()                    [Mapping - Application]
      → CreatedAtAction()                    [API - response 201]
```

---

## Patrones utilizados

| Patrón | Dónde se usa | Propósito |
|---|---|---|
| **Domain-Driven Design (DDD)** | 4 capas + Aggregate Root (`Product`) | Separar la lógica de negocio de la infraestructura |
| **Value Object (VO)** | `Price`, `ProductCode` | Objetos inmutables con validación propia, sin identidad |
| **Repository** | `IProductRepository` / `ProductRepository` | Abstraer acceso a datos |
| **Domain Service** | `ProductDomainService` | Lógica de negocio que involucra múltiples entidades o requiere acceso al repositorio |
| **Dependency Injection** | `AddInfrastructure()`, `Program.cs` | Inversión de control, desacoplamiento |
| **DTO** | `ProductDto`, `CreateProductDto`, `UpdateProductDto` | Separar modelo de dominio de la representación API |
| **Extension Methods** | `ProductMappingExtensions.ToDto()` | Mapeo manual sin AutoMapper |
| **Middleware** | `ExceptionMiddleware` | Manejo centralizado de excepciones → códigos HTTP |
| **Fluent Validation manual** | `CreateProductValidator`, `UpdateProductValidator` (static) | Validación síncrona de DTOs |
| **Result wrapper** | `PagedResult<T>` | Paginación con metadatos (total, páginas, navegación) |
| **Separated Interface** | `IProductService` + `ProductService` | Contrato separado de implementación |
| **Seed data** | `DbSeeder` | Datos iniciales de prueba |

---

## Flujo detallado por capa

### 1. Domain — El corazón del negocio

- `Product` (entidad) encapsula reglas: nombre no vacío, stock no negativo, precio validado.
- `Price` y `ProductCode` son `sealed record` inmutables que se validan en el constructor.
- `DomainException` se lanza ante violaciones de reglas de negocio.
- `ProductDomainService` orquesta operaciones multi-entidad (`TransferStock`) y verifica unicidad de código.
- `IProductRepository` es una interfaz en Domain (según DDD, el repositorio se declara en Domain pero se implementa en Infrastructure).

### 2. Application — Orquestación y coordinación

- `ProductService` usa el repositorio y el domain service para ejecutar casos de uso.
- Valida DTOs con validadores estáticos antes de delegar al dominio.
- Convierte entidades a DTOs con extension methods (sin AutoMapper).
- `PagedRequest` / `PagedResult` para paginación.

### 3. Infrastructure — Implementaciones concretas

- `AppDbContext` con EF Core InMemory. Ignora `Price` y `ProductCode` (EF no puede mapear records inmutables directamente), almacena primitivas (`CodeValue`, `PriceAmount`, `CurrencyCode`).
- `ProductRepository` implementa el repositorio con EF. Los VOs se reconstruyen desde las primitivas al leer (via getters de `Product`).
- `DbSeeder` precarga 10 productos de ejemplo.
- `DependencyInjection` registra DbContext, repositorio y domain service.

### 4. API — Contratos HTTP

- `ProductsController` con endpoints RESTful: `GET/POST/PUT/DELETE /api/products`, más `GET /active`, `GET /paged`, `PATCH /stock/add|remove`.
- `ExceptionMiddleware` captura `DomainException` → 400, `KeyNotFoundException` → 404, resto → 500.
- Swagger con XML docs y ejemplos en DTOs.

### 5. Tests — xUnit + Moq + FluentAssertions (64 tests)

- **Domain**: Prueban entidades y VOs directamente (sin mocks). Ej: `Product.Create_WithInvalidData_ShouldThrow`.
- **Application**: Mocks de `IProductRepository`, `ProductDomainService` real, prueban `ProductService`.
- **API**: Mocks de `IProductService`, prueban `IActionResult` del controller.

---

## Posibles mejoras

1. **Base de datos real**: EF Core InMemory no persiste datos. Migrar a SQLite/SQL Server con migraciones para producción.
2. **Validación con FluentValidation library**: En lugar de validadores `static` manuales, usar `FluentValidation` con inyección para mejor testabilidad y mensajes estandarizados.
3. **CQRS con MediatR**: Separar commands (CreateProduct) de queries (GetProduct) para mejor escalabilidad y separación de responsabilidades.
4. **Specification Pattern**: Para filtros complejos en repositorios (ej. filtrar por rango de precio + categoría).
5. **Unit of Work**: `SaveChangesAsync` se llama en cada operación del repositorio. Un Unit of Work permitiría transacciones multi-repositorio.
6. **AutoMapper vs Manual Mapping**: El mapeo manual es explícito pero verboso. Para proyectos grandes, AutoMapper o Mappingster reduce boilerplate.
7. **Logging estructurado**: Serilog/Sentry para mejor observabilidad.
8. **Health Checks**: Endpoint `/health` para monitoreo.
9. **Rate limiting**: Proteger la API contra abusos.
10. **Integración continua**: GitHub Actions con `dotnet build` + `dotnet test` automáticos.
11. **Caching**: Resultados de consultas frecuentes (ej. catálogo activo) con `IMemoryCache` o `IDistributedCache`.
12. **Global usings**: Centralizar `using` comunes en `GlobalUsings.cs` por proyecto.
13. **Result pattern**: En lugar de excepciones para flujo de control, usar `Result<T, Error>` para operaciones que pueden fallar (más predecible).
14. **Separar StockRequest**: La clase `StockRequest` está en el archivo del controller; moverla a DTOs.
15. **Agregar filtros/búsqueda**: endpoint con `?search=&minPrice=&maxPrice=&category=` usando Specification Pattern.
