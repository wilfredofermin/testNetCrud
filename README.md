# testNet

API RESTful para gestión de productos construida con **.NET 8** y **Domain-Driven Design**.

## Arquitectura

```
testNet.API          → Capa de presentación (Controllers, Middleware)
testNet.Application  → Casos de uso (Services, DTOs, Validators)
testNet.Domain       → Núcleo del negocio (Entities, ValueObjects, DomainServices)
testNet.Infrastructure → Persistencia (EF Core, Repositories)
```

### Dependencias
```
API → Application → Domain
API → Infrastructure → Application → Domain
```

## Funcionalidades

| Endpoint | Descripción |
|----------|-------------|
| `GET /api/products` | Lista todos los productos |
| `GET /api/products/active` | Lista solo productos activos |
| `GET /api/products/paged` | Lista con paginación (page, pageSize) |
| `GET /api/products/{id}` | Obtiene producto por ID |
| `GET /api/products/code/{code}` | Obtiene producto por código |
| `POST /api/products` | Crea un producto |
| `PUT /api/products/{id}` | Actualiza un producto |
| `DELETE /api/products/{id}` | Elimina un producto |
| `PATCH /api/products/{id}/stock/add` | Agrega stock |
| `PATCH /api/products/{id}/stock/remove` | Remueve stock |

## Cómo usar

```bash
# Restaurar dependencias
dotnet restore

# Ejecutar en desarrollo
dotnet run --project src/testNet.API

# Ejecutar tests
dotnet test

# Documentación Swagger
# Abrir http://localhost:5000/swagger
```

## Lógica aplicada

- **Value Objects inmutables** (Price, ProductCode): se validan a sí mismos al construirse, garantizando que nunca exista un estado inválido.
- **Domain Services**: encapsulan reglas de negocio que involucran múltiples entidades (ej: transferencia de stock entre productos).
- **Exception Middleware**: captura excepciones de dominio y las transforma en respuestas HTTP consistentes sin ensuciar los controladores.
- **Validación en Application**: los DTOs de entrada se validan antes de llegar al dominio, manteniendo las reglas de negocio puras.
- **Paginación**: todas las consultas list utilizan paginación para evitar saturación de memoria y red.
- **Mapeo explícito DTO ↔ Entidad**: evita exponer el modelo interno y permite evolucionar API y dominio independientemente.

## Mantenibilidad

- **Pruebas unitarias**: cada función del dominio, aplicación y API tiene su test correspondiente.
- **Separación de capas**: modificar persistencia no afecta reglas de negocio y viceversa.
- **Inyección de dependencias**: todas las dependencias se resuelven por constructor, facilitando mocking.
- **Código sin comentarios**: los nombres de clases, métodos y variables son autoexplicativos.

## Escalabilidad

- **Base de datos**: EF Core permite migrar de InMemory a SQL Server, PostgreSQL o CosmosDB cambiando el provider.
- **API stateless**: permite escalar horizontalmente con balanceadores de carga.
- **Paginación**: previene problemas de performance en colecciones grandes.
- **Arquitectura desacoplada**: cada capa puede escalarse independientemente.

## Estructura del proyecto

```
├── src/
│   ├── testNet.API/            # Controladores y middleware
│   ├── testNet.Application/    # Casos de uso y DTOs
│   ├── testNet.Domain/         # Entidades y reglas de negocio
│   └── testNet.Infrastructure/ # EF Core y repositorios
├── tests/
│   └── testNet.Tests/          # Tests unitarios (xUnit + Moq)
├── docs/
│   ├── ADR.md                  # Decisiones arquitectónicas
│   ├── SPEC.md                 # Especificación técnica
│   ├── endpoints.md            # Documentación de endpoints
│   └── collection.json         # Colección Postman
├── AGENT.md                    # Contexto para asistentes IA
└── README.md
```
