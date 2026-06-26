# SPEC - testNet Project

## 1. Visión General

Sistema CRUD de productos construido con **.NET 8** siguiendo principios de **Domain-Driven Design (DDD)**. Proporciona una API RESTful para la gestión completa del ciclo de vida de productos, incluyendo control de stock, precios y estados.

## 2. Stack Tecnológico

| Componente        | Tecnología                          |
|-------------------|-------------------------------------|
| Runtime           | .NET 8                              |
| Lenguaje          | C# 12                               |
| ORM               | Entity Framework Core 8             |
| Base de datos     | InMemory (intercambiable por SQL Server/PostgreSQL) |
| Documentación API | Swagger UI + Redoc (OpenAPI 3.0)     |
| Testing           | xUnit + Moq + FluentAssertions + WebApplicationFactory |

## 3. Arquitectura

```
├── testNet.Domain/           # Capa de dominio (núcleo)
│   ├── Entities/             # Entidades del negocio
│   ├── ValueObjects/         # Objetos de valor inmutables
│   ├── Interfaces/           # Contratos de repositorio
│   ├── DomainServices/       # Reglas de negocio complejas
│   └── Exceptions/           # Excepciones de dominio
│
├── testNet.Application/      # Casos de uso de la aplicación
│   ├── DTOs/                 # Objetos de transferencia
│   ├── Interfaces/           # Contratos de servicios
│   ├── Services/             # Implementación de casos de uso
│   ├── Validators/           # Validación de entrada
│   └── Common/               # Tipos compartidos (Paginación)
│
├── testNet.Infrastructure/   # Persistencia e infraestructura
│   ├── Data/                 # DbContext y configuraciones EF
│   └── Repositories/         # Implementación de repositorios
│
└── testNet.API/              # Capa de presentación
    ├── Controllers/          # Endpoints REST
    └── Middleware/            # Manejo global de errores
```

## 4. Patrones Aplicados

### 4.1 Domain-Driven Design (DDD)
- **Entidades**: `Product` con identidad única (Guid)
- **Value Objects**: `Price`, `ProductCode` (inmutables, auto-validados)
- **Domain Services**: `ProductDomainService` para reglas que abarcan múltiples entidades
- **Repositories**: Abstracciones en Domain, implementaciones en Infrastructure

### 4.2 Principios SOLID
- **S**: Cada clase tiene una responsabilidad única
- **O**: Extensible via interfaces (IProductRepository, IProductService)
- **L**: Las implementaciones son sustituibles
- **I**: Interfaces segregadas y específicas
- **D**: Domain no depende de Infrastructure

### 4.3 Otros Patrones
- **Repository Pattern**: Abstracción de persistencia
- **Dependency Injection**: Inversión de control nativa de .NET
- **Result Pattern**: Manejo consistente de errores
- **Middleware Pipeline**: Exception middleware global
- **Data Annotations**: `[Required]`, `[Range]`, `[StringLength]`, `[DefaultValue]` en DTOs para enriquecer schemas Swagger/Redoc

## 5. Modelo de Dominio

### Product
```
Id            : Guid          (identidad única)
Code          : ProductCode   (VO, único, max 50 chars)
Name          : string        (requerido, max 200 chars)
Description   : string        (opcional, max 2000 chars)
UnitPrice     : Price         (VO: Amount+Currency)
StockQuantity : int           (>= 0)
IsActive      : bool
CreatedAt     : DateTime
UpdatedAt     : DateTime?
```

### Reglas de Negocio
- El código de producto debe ser único en el sistema
- El stock nunca puede ser negativo
- No se puede remover más stock del disponible
- El precio no puede ser negativo
- Los nombres no pueden estar vacíos
- Los objetos de valor son inmutables y se validan al crearse

## 6. API Endpoints

| Método | Ruta                     | Descripción                |
|--------|--------------------------|----------------------------|
| GET    | /api/products            | Listar todos los productos |
| GET    | /api/products/active     | Listar productos activos   |
| GET    | /api/products/paged      | Listar con paginación      |
| GET    | /api/products/{id}       | Obtener por ID             |
| GET    | /api/products/code/{code}| Obtener por código         |
| POST   | /api/products            | Crear producto             |
| PUT    | /api/products/{id}       | Actualizar producto        |
| DELETE | /api/products/{id}       | Eliminar producto          |
| PATCH  | /api/products/{id}/stock/add     | Agregar stock    |
| PATCH  | /api/products/{id}/stock/remove  | Remover stock    |
| POST   | /api/seed                        | Resetear y sembrar DB con 10 productos de prueba |

## 7. Escalabilidad y Mantenibilidad

### Escalabilidad
- Arquitectura desacoplada permite escalar capas independientemente
- EF Core con InMemory es transicionable a SQL Server/PostgreSQL/CosmosDB
- Stateless API permite balanceo de carga horizontal
- Paginación implementada para evitar sobrecarga

### Mantenibilidad
- Validación centralizada en Value Objects y Validators
- Middleware de errores global evita try-catch repetitivos
- DTOs separados de entidades de dominio
- Test unitarios por cada función crítica
- Inyección de dependencias facilita mocking y testing
- Código autodocumentado con nombres explícitos

## 8. Pruebas

### 8.1 Objetivo de cobertura

La suite de pruebas debe mantener una **cobertura mínima del 98%** sobre el código de las capas de dominio, aplicación, infraestructura y presentación. El umbral se evalúa con `coverlet.collector` (formato Cobertura) y debe verificarse antes de considerar válida una Contribución al proyecto.

| Métrica       | Umbral mínimo |
|---------------|---------------|
| Cobertura de líneas (`line-rate`)   | **≥ 98%** |
| Cobertura de ramas (`branch-rate`)  | **≥ 98%** |

Comando de verificación:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### 8.2 Distribución de tests

```
tests/testNet.Tests/
├── Domain/
│   ├── ProductTests.cs                 # 13 tests
│   ├── ProductCodeTests.cs             # 5 tests
│   ├── PriceTests.cs                   # 6 tests
│   ├── ProductDomainServiceTests.cs    # 3 tests
│   └── DomainExceptionTests.cs         # 2 tests (constructor mensaje y excepción interna)
├── Application/
│   ├── ProductServiceTests.cs          # 15 tests
│   ├── ValidatorsTests.cs              # 3 tests
│   ├── ProductMappingTests.cs          # 1 test
│   ├── PagedResultTests.cs             # 6 tests (TotalPages, HasPreviousPage, HasNextPage)
│   └── DtoValidationAnnotationsTests.cs # 22 tests (CreateProduct, UpdateProduct, PagedRequest, StockRequest)
├── Infrastructure/
│   ├── ProductRepositoryTests.cs       # 12 tests (CRUD y consultas sobre AppDbContext InMemory)
│   └── DbSeederTests.cs                # 3 tests (SeedAsync, ResetAndSeedAsync)
└── API/
    ├── ProductsControllerTests.cs      # 10 tests
    ├── ExceptionMiddlewareTests.cs     # 4 tests (DomainException, KeyNotFoundException, excepción inesperada)
    └── IntegrationTests.cs              # 4 tests (Redoc, Swagger y Seed endpoint)
```

**Total: 135 tests — Cobertura: 99.63% líneas, 100% ramas.** Validación de dominio, servicios de aplicación, mapeo DTO, controladores, repositorios, middleware de excepciones, seeder de base de datos, anotaciones de datos, documentación interactiva y seed endpoint.
