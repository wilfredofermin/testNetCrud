# Endpoint Documentation

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
