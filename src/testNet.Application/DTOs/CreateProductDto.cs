namespace testNet.Application.DTOs;

/// <summary>Datos necesarios para crear un nuevo producto.</summary>
public class CreateProductDto
{
    /// <summary>Código único del producto. No debe existir previamente en el sistema.</summary>
    /// <example>LAP-003</example>
    public string Code { get; set; } = string.Empty;

    /// <summary>Nombre del producto.</summary>
    /// <example>Laptop Ultrabook i9</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>Descripción del producto.</summary>
    /// <example>Ultrabook con procesador i9, 16GB RAM, pantalla táctil</example>
    public string Description { get; set; } = string.Empty;

    /// <summary>Precio unitario. No puede ser negativo.</summary>
    /// <example>1899.99</example>
    public decimal Price { get; set; }

    /// <summary>Cantidad inicial en inventario. No puede ser negativa.</summary>
    /// <example>20</example>
    public int StockQuantity { get; set; }

    /// <summary>Código de moneda ISO 4217 (por defecto USD).</summary>
    /// <example>USD</example>
    public string Currency { get; set; } = "USD";
}
