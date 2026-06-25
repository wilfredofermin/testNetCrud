namespace testNet.Application.DTOs;

/// <summary>Datos para actualizar un producto existente.</summary>
public class UpdateProductDto
{
    /// <summary>Nuevo nombre del producto.</summary>
    /// <example>Laptop Ultrabook i9 Pro</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>Nueva descripción.</summary>
    /// <example>Ultrabook con procesador i9, 32GB RAM, pantalla táctil 4K</example>
    public string Description { get; set; } = string.Empty;

    /// <summary>Nuevo precio unitario.</summary>
    /// <example>2199.99</example>
    public decimal Price { get; set; }

    /// <summary>Nueva cantidad en inventario.</summary>
    /// <example>25</example>
    public int StockQuantity { get; set; }

    /// <summary>Nuevo código de moneda.</summary>
    /// <example>USD</example>
    public string Currency { get; set; } = "USD";

    /// <summary>Estado activo/inactivo del producto.</summary>
    /// <example>true</example>
    public bool IsActive { get; set; }
}
