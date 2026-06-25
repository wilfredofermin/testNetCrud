using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace testNet.Application.DTOs;

/// <summary>Representación de un producto para la API.</summary>
public class ProductDto
{
    /// <summary>Identificador único del producto.</summary>
    /// <example>3f7b9c8a-2d1e-4f6a-b5c3-8d9e0f1a2b3c</example>
    public Guid Id { get; set; }

    /// <summary>Código único del producto (ej: LAP-001).</summary>
    /// <example>LAP-001</example>
    [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters.")]
    public string Code { get; set; } = string.Empty;

    /// <summary>Nombre del producto.</summary>
    /// <example>Laptop Gamer RTX 4080</example>
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Descripción detallada del producto.</summary>
    /// <example>Laptop de alta gama con NVIDIA RTX 4080, 32GB RAM, 1TB SSD</example>
    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Precio unitario del producto.</summary>
    /// <example>2499.99</example>
    [Range(0, (double)decimal.MaxValue, ErrorMessage = "Price cannot be negative.")]
    public decimal Price { get; set; }

    /// <summary>Código de moneda (ISO 4217).</summary>
    /// <example>USD</example>
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a 3-letter ISO 4217 code.")]
    [DefaultValue("USD")]
    public string Currency { get; set; } = "USD";

    /// <summary>Cantidad disponible en inventario.</summary>
    /// <example>15</example>
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
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
