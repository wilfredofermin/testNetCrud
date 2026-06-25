using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace testNet.Application.DTOs;

/// <summary>Datos para actualizar un producto existente.</summary>
public class UpdateProductDto
{
    /// <summary>Nuevo nombre del producto.</summary>
    /// <example>Laptop Ultrabook i9 Pro</example>
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 200 characters.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Nueva descripción. Máximo 2000 caracteres.</summary>
    /// <example>Ultrabook con procesador i9, 32GB RAM, pantalla táctil 4K</example>
    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Nuevo precio unitario. No puede ser negativo.</summary>
    /// <example>2199.99</example>
    [Range(0, (double)decimal.MaxValue, ErrorMessage = "Price cannot be negative.")]
    public decimal Price { get; set; }

    /// <summary>Nueva cantidad en inventario. No puede ser negativa.</summary>
    /// <example>25</example>
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
    public int StockQuantity { get; set; }

    /// <summary>Nuevo código de moneda ISO 4217.</summary>
    /// <example>USD</example>
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a 3-letter ISO 4217 code.")]
    [DefaultValue("USD")]
    public string Currency { get; set; } = "USD";

    /// <summary>Estado activo/inactivo del producto.</summary>
    /// <example>true</example>
    public bool IsActive { get; set; }
}
