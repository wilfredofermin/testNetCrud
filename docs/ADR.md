# ADR - Architecture Decision Records

## ADR-001: Adopción de Domain-Driven Design (DDD)

**Estado**: Aceptado  
**Contexto**: Necesitamos una arquitectura que aisle las reglas de negocio de los detalles de infraestructura, permitiendo que el dominio sea testeable y evolucione independientemente.  
**Decisión**: Implementar DDD con 4 capas: Domain, Application, Infrastructure, API.  
**Consecuencias**:
- El dominio no tiene dependencias externas
- La lógica de negocio está centralizada y es fácil de testear
- Mayor cantidad de archivos/código inicial, pero más mantenible a largo plazo
- Curva de aprendizaje para developers no familiarizados con DDD

## ADR-002: Uso de Value Objects para Price y ProductCode

**Estado**: Aceptado  
**Contexto**: Precio y código de producto tienen reglas de validación y forman conceptos atómicos en el dominio.  
**Decisión**: Modelar como `sealed record` inmutables con autovalidación en el constructor.  
**Consecuencias**:
- Garantiza estado válido siempre
- Elimina validación dispersa en servicios
- Facilidad de testing y mantenimiento
- Los records de C# 10+ proporcionan igualdad estructural

## ADR-003: InMemory Database para Desarrollo y Testing

**Estado**: Aceptado  
**Contexto**: Necesitamos una base de datos para desarrollo que no requiera instalación de servidores.  
**Decisión**: Usar EF Core InMemory Database.  
**Consecuencias**:
- Rápido setup, sin dependencias externas
- Los tests son deterministas
- La transición a SQL Server/PostgreSQL es trivial (cambiar connection string y provider)
- InMemory no soporta transacciones reales ni migraciones
- No es adecuado para producción

## ADR-004: Exception Middleware Global

**Estado**: Aceptado  
**Contexto**: El manejo de errores debe ser consistente en toda la API, evitando try-catch repetitivos en cada endpoint.  
**Decisión**: Implementar middleware personalizado que capture DomainException → 400, KeyNotFoundException → 404, Exception → 500.  
**Consecuencias**:
- Código de controladores más limpio
- Formato de error JSON consistente
- Logging centralizado
- Fácil extender para nuevos tipos de error

## ADR-005: Separación DTOs / Entidades de Dominio

**Estado**: Aceptado  
**Contexto**: Las entidades de dominio contienen comportamiento y reglas de negocio que no deben exponerse directamente en la API.  
**Decisión**: Crear DTOs específicos para cada operación (CreateProductDto, UpdateProductDto, ProductDto) con mapeo explícito via extension methods.  
**Consecuencias**:
- La API no expone detalles internos del dominio
- Los DTOs pueden evolucionar independientemente
- Mayor cantidad de código de mapeo pero sin overhead de librerías externas
- Serialización controlada (evita loops de referencia, expone solo campos necesarios)

## ADR-006: Paginación en Consultas List

**Estado**: Aceptado  
**Contexto**: Las colecciones de productos pueden crecer significativamente y afectar performance.  
**Decisión**: Implementar paginación con page/pageSize + metadata de navegación.  
**Consecuencias**:
- Protege contra sobrecarga de datos
- Cliente conoce total de páginas y navegación
- Fácil migrar a cursor-based pagination si es necesario
