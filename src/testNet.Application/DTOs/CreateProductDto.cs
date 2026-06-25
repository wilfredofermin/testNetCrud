using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace testNet.Application.DTOs;

/// <summary>Datos necesarios para crear un nuevo producto.</summary>
public class CreateProductDto
{
    /// <summary>Código único del producto. No debe existir previamente en el sistema.</summary>
    /// <example>LAP-003</example>
    [Required(ErrorMessage = "Code is required.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Code must be between 1 and 50 characters.")]
    public string Code { get; set; } = string.Empty;

    /// <summary>Nombre del producto.</summary>
    /// <example>Laptop Ultrabook i9</example>
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 200 characters.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Descripción del producto. Máximo 2000 caracteres.</summary>
    /// <example>Ultrabook con procesador i9, 16GB RAM, pantalla táctil</example>
    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Precio unitario. No puede ser negativo.</summary>
    /// <example>1899.99</example>
    [Range(0, (double)decimal.MaxValue, ErrorMessage = "Price cannot be negative.")]
    public decimal Price { get; set; }

    /// <summary>Cantidad inicial en inventario. No puede ser negativa.</summary>
    /// <example>20</example>
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
    public int StockQuantity { get; set; }

    /// <summary>Código de moneda ISO 4217 (por defecto USD).</summary>
    /// <example>USD</example>
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a 3-letter ISO 4217 code.")]
    [DefaultValue("USD")]
    public string Currency { get; set; } = "USD";
}
