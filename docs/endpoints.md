# Endpoint Documentation

## Documentación interactiva

Además de este documento estático, la API expone dos herramientas de documentación dinámica:

| Herramienta | URL | Descripción |
|---|---|---|
| **Swagger UI** | `http://localhost:5106/swagger` | Interfaz interactiva para explorar y probar endpoints |
| **Redoc** | `http://localhost:5106/redoc` | Documentación visual de lectura limpia en columnas |

Ambas se generan desde el mismo archivo `swagger.json` (OpenAPI 3.0) y reflejan los schemas con **data annotations** (`[Required]`, `[Range]`, `[StringLength]`, `[DefaultValue]`) que muestran constraints y ejemplos en cada campo.

## Postman

Importar `docs/collection.json` en Postman. La colección incluye:

- Variable `baseUrl` → `http://localhost:5106`
- Variable `productId` → se actualiza automáticamente al crear un producto
- Todos los endpoints CRUD + stock preconfigurados

```json
// Ejemplo: Body de POST /api/products en Postman
{
  "code": "LAP-010",
  "name": "Nuevo Producto",
  "description": "Descripción del producto",
  "price": 599.99,
  "stockQuantity": 25,
  "currency": "USD"
}
```

## Product CRUD - Casos de Uso

---

### UC-01: Listar Todos los Productos

```
GET /api/products
```

**Caso de Uso**: Obtener el catálogo completo de productos.  
**Response**: `200 OK`

```json
[
  {
    "id": "guid",
    "code": "PRD-001",
    "name": "Laptop Gamer",
    "description": "Laptop de alta gama",
    "price": 1500.00,
    "currency": "USD",
    "stockQuantity": 10,
    "isActive": true,
    "createdAt": "2026-01-01T00:00:00Z",
    "updatedAt": null
  }
]
```

---

### UC-02: Listar Productos Activos

```
GET /api/products/active
```

**Caso de Uso**: Obtener solo productos disponibles para la venta.  
**Response**: `200 OK` (array de ProductDto)

---

### UC-03: Listar Productos con Paginación

```
GET /api/products/paged?page=1&pageSize=10
```

**Caso de Uso**: Navegar el catálogo con control de cantidad de resultados.  
**Response**: `200 OK`

```json
{
  "items": [ ... ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 50,
  "totalPages": 5,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

---

### UC-04: Obtener Producto por ID

```
GET /api/products/{id}
```

**Caso de Uso**: Consultar detalle de un producto específico.  
**Response**: `200 OK` (ProductDto) | `404 Not Found`

---

### UC-05: Obtener Producto por Código

```
GET /api/products/code/{code}
```

**Caso de Uso**: Buscar producto por código único (ej: "PRD-001").  
**Response**: `200 OK` (ProductDto) | `404 Not Found`

---

### UC-06: Crear Producto

```
POST /api/products
```

**Caso de Uso**: Registrar un nuevo producto en el sistema.  
**Request Body**:

```json
{
  "code": "PRD-001",
  "name": "Laptop Gamer",
  "description": "Laptop de alta gama con RTX 4080",
  "price": 1500.00,
  "stockQuantity": 10,
  "currency": "USD"
}
```

**Response**: `201 Created` (ProductDto + Location header)  
**Error**: `400 Bad Request` (código duplicado, datos inválidos)

---

### UC-07: Actualizar Producto

```
PUT /api/products/{id}
```

**Caso de Uso**: Modificar datos de un producto existente.  
**Request Body**:

```json
{
  "name": "Laptop Gamer Pro",
  "description": "Versión mejorada",
  "price": 1800.00,
  "stockQuantity": 15,
  "currency": "USD",
  "isActive": true
}
```

**Response**: `200 OK` (ProductDto) | `404 Not Found`

---

### UC-08: Eliminar Producto

```
DELETE /api/products/{id}
```

**Caso de Uso**: Remover un producto del sistema.  
**Response**: `204 No Content` | `404 Not Found`

---

### UC-09: Agregar Stock

```
PATCH /api/products/{id}/stock/add
```

**Caso de Uso**: Incrementar el inventario de un producto (reposición).  
**Request Body**:

```json
{
  "quantity": 50
}
```

**Response**: `200 OK` (ProductDto actualizado) | `404 Not Found`

---

---

### UC-11: Resetear y Sembrar Base de Datos

```
POST /api/seed
```

**Caso de Uso**: Eliminar todos los productos y recargar los 10 productos de prueba. Útil para desarrollo y testing.  
**Response**: `200 OK`

```json
{
  "message": "Database reset and seeded with 10 products.",
  "count": 10
}
```

### UC-10: Remover Stock

```
PATCH /api/products/{id}/stock/remove
```

**Caso de Uso**: Decrementar el inventario (venta, daño, etc.).  
**Request Body**:

```json
{
  "quantity": 5
}
```

**Response**: `200 OK` (ProductDto actualizado)  
**Error**: `400 Bad Request` (stock insuficiente)
