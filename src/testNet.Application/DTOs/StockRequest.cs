using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace testNet.Application.DTOs;

/// <summary>Solicitud para operaciones de ajuste de stock.</summary>
public class StockRequest
{
    /// <summary>Cantidad de unidades a agregar o remover. Debe ser un valor positivo.</summary>
    /// <example>10</example>
    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive value.")]
    public int Quantity { get; set; }
}
