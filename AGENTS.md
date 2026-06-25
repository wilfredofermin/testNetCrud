# AGENTS.md — testNet

## INSTRUCCIÓN PARA EL AGENTE

Al inicializar, DEBES leer los siguientes archivos para comprender el proyecto:

1. **`docs/SPEC.md`** — Especificación técnica completa: stack, arquitectura, modelo de dominio, reglas de negocio, principios SOLID, escalabilidad, y distribución de tests. Es la fuente principal de contexto.
2. **`docs/endpoints.md`** — Documentación de cada endpoint con ejemplos JSON de request/response.
3. **`docs/ADR.md`** — Decisiones arquitectónicas y su justificación.

La información contenida en este `AGENTS.md` es un resumen ejecutivo para arranque rápido. Ante cualquier duda de diseño o comportamiento, pri riza la información de `docs/SPEC.md`.

---

## RESUMEN EJECUTIVO

API RESTful de gestión de productos sobre .NET 8 con Domain-Driven Design en 4 capas estrictas.

```
API → Infrastructure → Application → Domain
```

**Stack**: .NET 8 | C# 12 | EF Core 8 (InMemory) | Swashbuckle 6 | Redoc
**Tests**: 90 — xUnit + Moq + FluentAssertions + WebApplicationFactory

**Regla fundamental**: NO agregar dependencias externas innecesarias (cero AutoMapper, cero FluentValidation, cero MediatR).

**Validación doble capa**: (1) Data annotations `[Required][Range][StringLength]` en DTOs para validación automática ASP.NET, (2) validadores manuales `CreateProductValidator.Validate()` para reglas de negocio.

---

## COMANDOS

```bash
dotnet build                  # 0 warnings requerido
dotnet test                   # 90 tests
dotnet run --project src/testNet.API --urls "http://localhost:5106"  # http://localhost:5106/swagger
```

---

## ENDPOINTS

| Método | Ruta | Response |
|--------|------|----------|
| GET | `/api/products` | `200` → lista |
| GET | `/api/products/active` | `200` → activos |
| GET | `/api/products/paged?page=&pageSize=` | `200` → paginado |
| GET | `/api/products/{id:guid}` | `200` / `404` |
| GET | `/api/products/code/{code}` | `200` / `404` |
| POST | `/api/products` | `201` / `400` |
| PUT | `/api/products/{id:guid}` | `200` / `400` / `404` |
| DELETE | `/api/products/{id:guid}` | `204` / `404` |
| PATCH | `/api/products/{id:guid}/stock/add` | `200` / `400` / `404` |
| PATCH | `/api/products/{id:guid}/stock/remove` | `200` / `400` / `404` |
| POST | `/api/seed` | `200` → resetea datos |

---

## FLUJO TÍPICO

```
POST /api/products
  → ExceptionMiddleware
    → ProductsController.Create()
      → IProductService.CreateAsync()
        → CreateProductValidator.Validate()
        → ProductDomainService.CreateProductAsync() [unicidad]
          → IProductRepository.ExistsByCodeAsync()
          → new Product()
        → IProductRepository.AddAsync()
      → Product.ToDto()
    → CreatedAtAction()
```
