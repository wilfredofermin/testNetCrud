# testNet

API RESTful de gestión de productos sobre **.NET 8** con arquitectura **Domain-Driven Design** en 4 capas estrictas. Implementa CRUD completo con 11 endpoints, control de inventario, paginación con metadatos y Value Objects inmutables. Documentación interactiva via Swagger UI y Redoc. Persistencia con EF Core InMemory (intercambiable por SQL Server/PostgreSQL). **90 tests** (xUnit + Moq + FluentAssertions + WebApplicationFactory). Sin dependencias externas más allá de EF Core y Swashbuckle — cero AutoMapper, cero FluentValidation.

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
| `POST /api/seed` | Resetea y siembra DB con 10 productos de prueba |

## Cómo usar

```bash
# Restaurar dependencias
dotnet restore

# Ejecutar en desarrollo
dotnet run --project src/testNet.API --urls "http://localhost:5106"

# Ejecutar tests
dotnet test

# Documentación interactiva
# Swagger UI: http://localhost:5106/swagger
# Redoc:      http://localhost:5106/redoc
```

## Documentación interactiva

El proyecto incluye dos herramientas de documentación API basadas en OpenAPI 3.0:

### Swagger UI (`/swagger`)

Interfaz clásica que permite explorar endpoints y ejecutar peticiones directamente desde el navegador. Cada DTO incluye **data annotations** (`[Required]`, `[Range]`, `[StringLength]`, `[DefaultValue]`) que Swagger renderiza como constraints en los schemas:

- Campos requeridos marcados con asterisco rojo
- Límites de longitud en strings (min/max)
- Rangos numéricos (min/max)
- Valores por defecto visibles

### Redoc (`/redoc`)

Interfaz alternativa con diseño limpio y columnas, ideal para compartir como documentación estática. Generada desde el mismo `swagger.json`, muestra todos los endpoints, schemas y ejemplos en un formato de lectura más amigable.

## Lógica aplicada

- **Value Objects inmutables** (Price, ProductCode): se validan a sí mismos al construirse, garantizando que nunca exista un estado inválido.
- **Domain Services**: encapsulan reglas de negocio que involucran múltiples entidades (ej: transferencia de stock entre productos).
- **Exception Middleware**: captura excepciones de dominio y las transforma en respuestas HTTP consistentes sin ensuciar los controladores.
- **Validación en Application**: los DTOs se validan en dos niveles: (1) data annotations para constraints básicos en la capa API, (2) validadores manuales para reglas de negocio antes de llegar al dominio.
- **Paginación**: todas las consultas list utilizan paginación para evitar saturación de memoria y red.
- **Mapeo explícito DTO ↔ Entidad**: evita exponer el modelo interno y permite evolucionar API y dominio independientemente.

## Postman

La colección de Postman se encuentra en `docs/collection.json`. Para usarla:

1. Abre Postman → **Import** → selecciona `docs/collection.json`
2. La colección incluye una variable `baseUrl` configurada como `http://localhost:5106`
3. Ejecuta **Create Product** primero (guarda automáticamente el `productId` como variable de colección)
4. Los demás endpoints usan `{{baseUrl}}` y `{{productId}}` automáticamente

```json
// Ejemplo: crear producto desde Postman
POST {{baseUrl}}/api/products
Content-Type: application/json

{
  "code": "LAP-010",
  "name": "Nuevo Producto",
  "description": "Descripción del producto",
  "price": 599.99,
  "stockQuantity": 25,
  "currency": "USD"
}
```

## Mantenibilidad

- **Pruebas unitarias**: 88 tests (dominio, aplicación, API + validación de anotaciones e integración).
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
│   ├── testNet.API/            # Controladores, middleware y endpoints
│   ├── testNet.Application/    # Casos de uso, DTOs, validadores
│   ├── testNet.Domain/         # Entidades y reglas de negocio
│   └── testNet.Infrastructure/ # EF Core y repositorios
├── tests/
│   └── testNet.Tests/          # Tests (xUnit + Moq + FluentAssertions)
├── docs/
│   ├── ADR.md                  # Decisiones arquitectónicas
│   ├── SPEC.md                 # Especificación técnica
│   ├── endpoints.md            # Documentación de endpoints
│   ├── ARCHITECTURE.md         # Explicación detallada de patrones
│   └── collection.json         # Colección Postman
├── AGENTS.md                   # Contexto para asistentes IA
└── README.md

## Asistencia por IA (Agentes)

El archivo `AGENTS.md` en la raíz del proyecto funciona como **contexto de entrada para asistentes de IA** (como opencode, Claude, ChatGPT, etc.). Contiene:

- **Arquitectura del proyecto**: ubicación de cada capa y sus dependencias
- **Comandos exactos**: `dotnet build`, `dotnet test`, `dotnet run` con los flags correctos
- **Puntos clave**: reglas sobre Value Objects, mapeo DTO, Exception Middleware, restricciones de EF Core InMemory
- **Patrones de tests**: convención de nombres, nivel de mocking por capa, helpers reutilizables
- **Swagger**: estándar de documentación XML en endpoints y DTOs

Al cargar `AGENTS.md` como instrucción inicial, el asistente IA conoce la estructura, comandos y convenciones del proyecto sin necesidad de explicación manual, lo que acelera tareas de mantenimiento, generación de código y debugging.
```
