using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace testNet.Application.Common;

/// <summary>Parámetros de paginación para consultas.</summary>
public class PagedRequest
{
    /// <summary>Número de página (1-based).</summary>
    /// <example>1</example>
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
    [DefaultValue(1)]
    public int Page { get; set; } = 1;

    /// <summary>Cantidad de elementos por página (1-100).</summary>
    /// <example>10</example>
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
    [DefaultValue(10)]
    public int PageSize { get; set; } = 10;
}
