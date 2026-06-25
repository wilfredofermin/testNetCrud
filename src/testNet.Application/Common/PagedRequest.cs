namespace testNet.Application.Common;

/// <summary>Parámetros de paginación para consultas.</summary>
public class PagedRequest
{
    /// <summary>Número de página (1-based).</summary>
    /// <example>1</example>
    public int Page { get; set; } = 1;

    /// <summary>Cantidad de elementos por página.</summary>
    /// <example>10</example>
    public int PageSize { get; set; } = 10;
}
