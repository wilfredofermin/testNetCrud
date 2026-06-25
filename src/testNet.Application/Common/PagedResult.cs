namespace testNet.Application.Common;

/// <summary>Resultado paginado con metadatos de navegación.</summary>
public class PagedResult<T>
{
    /// <summary>Elementos de la página actual.</summary>
    public IEnumerable<T> Items { get; set; } = [];

    /// <summary>Número de página actual.</summary>
    /// <example>1</example>
    public int Page { get; set; }

    /// <summary>Tamaño de página.</summary>
    /// <example>10</example>
    public int PageSize { get; set; }

    /// <summary>Total de elementos en toda la colección.</summary>
    /// <example>100</example>
    public int TotalCount { get; set; }

    /// <summary>Total de páginas disponibles.</summary>
    /// <example>10</example>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>Indica si existe una página anterior.</summary>
    /// <example>false</example>
    public bool HasPreviousPage => Page > 1;

    /// <summary>Indica si existe una página siguiente.</summary>
    /// <example>true</example>
    public bool HasNextPage => Page < TotalPages;
}
