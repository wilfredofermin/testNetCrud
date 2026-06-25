namespace testNet.Application.DTOs;

/// <summary>Representación de un producto para la API.</summary>
public class ProductDto
{
    /// <summary>Identificador único del producto.</summary>
    /// <example>3f7b9c8a-2d1e-4f6a-b5c3-8d9e0f1a2b3c</example>
    public Guid Id { get; set; }

    /// <summary>Código único del producto (ej: LAP-001).</summary>
    /// <example>LAP-001</example>
    public string Code { get; set; } = string.Empty;

    /// <summary>Nombre del producto.</summary>
    /// <example>Laptop Gamer RTX 4080</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>Descripción detallada del producto.</summary>
    /// <example>Laptop de alta gama con NVIDIA RTX 4080, 32GB RAM, 1TB SSD</example>
    public string Description { get; set; } = string.Empty;

    /// <summary>Precio unitario del producto.</summary>
    /// <example>2499.99</example>
    public decimal Price { get; set; }

    /// <summary>Código de moneda (ISO 4217).</summary>
    /// <example>USD</example>
    public string Currency { get; set; } = "USD";

    /// <summary>Cantidad disponible en inventario.</summary>
    /// <example>15</example>
    public int StockQuantity { get; set; }

    /// <summary>Indica si el producto está activo para la venta.</summary>
    /// <example>true</example>
    public bool IsActive { get; set; }

    /// <summary>Fecha de creación del registro.</summary>
    /// <example>2026-06-25T12:00:00Z</example>
    public DateTime CreatedAt { get; set; }

    /// <summary>Fecha de última actualización.</summary>
    /// <example>2026-06-25T14:30:00Z</example>
    public DateTime? UpdatedAt { get; set; }
}
