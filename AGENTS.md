# AGENTS.md — testNet

.NET 8 REST API con Domain-Driven Design (4 capas).

## Arquitectura

```
src/testNet.Domain/        # Entidades, ValueObjects, DomainServices, interfaces de repositorio
src/testNet.Application/   # Services, DTOs, Validators, Common (PagedResult)
src/testNet.Infrastructure/ # EF Core (InMemory), Repositorios, DbSeeder, DependencyInjection
src/testNet.API/           # Controllers, ExceptionMiddleware, Swagger
tests/testNet.Tests/       # xUnit + Moq + FluentAssertions
```

Dependencias entre capas (estrictas): `API → Infrastructure → Application → Domain`. Domain no depende de nada.

## Comandos exactos

```bash
dotnet build                  # 0 warnings requerido
dotnet test                   # 64 tests, ~120ms
dotnet run --project src/testNet.API --urls "http://localhost:5106"  # API + Swagger + seeder
```

## Puntos clave

- **Value Objects inmutables** (`Price`, `ProductCode`) — se validan en el constructor, `sealed record`.
- **Mapeo DTO manual** por extension methods (`ProductMappingExtensions.ToDto()`), sin AutoMapper.
- **Exception Middleware** captura `DomainException` → 400, `KeyNotFoundException` → 404, resto → 500.
- **DbContext** ignora `Price`/`ProductCode` globalmente; los VO se construyen desde primitivas (`CodeValue`, `PriceAmount`, `CurrencyCode`).
- **EF Core InMemory** — no soporta `HasColumnName`, `HasDefaultValue`, `HasPrecision`; solo usar configuraciones InMemory-compatibles.
- **Seeder** ejecuta en startup (`Program.cs` línea 53). Si `Products.Any()` salta.

## Tests — patrones

- Nombres: `{Metodo}_{Escenario}_Should{Comportamiento}`
- Domain: no mocks, assert sobre entidades directamente
- Application: `Mock<IProductRepository>` + `ProductDomainService` real (no mockeado) + `ProductService`
- API: `Mock<IProductService>`, assert sobre `IActionResult`
- Fixtures: helpers privados `CreateSampleProduct()`, `CreateSampleCreateDto()`

## Swagger

- Endpoints con XML `<summary>`, `<remarks>`, `<param>`, `<response>`
- DTOs con `<example>` en cada propiedad
- `NoWarn 1591,1573` para no documentar CancellationToken o parámetros triviales
