using Microsoft.AspNetCore.Mvc;
using testNet.Application.Common;
using testNet.Application.DTOs;
using testNet.Application.Interfaces;

namespace testNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>Obtiene todos los productos del catálogo.</summary>
    /// <remarks>Retorna la lista completa de productos registrados, incluyendo activos e inactivos.</remarks>
    /// <response code="200">Lista de productos</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var products = await _productService.GetAllAsync(cancellationToken);
        return Ok(products);
    }

    /// <summary>Obtiene solo los productos activos.</summary>
    /// <remarks>Filtra y retorna únicamente productos marcados como activos (IsActive = true).</remarks>
    /// <response code="200">Lista de productos activos</response>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
    {
        var products = await _productService.GetActiveAsync(cancellationToken);
        return Ok(products);
    }

    /// <summary>Obtiene productos paginados.</summary>
    /// <remarks>Retorna una página de productos con metadatos de paginación (total, páginas, navegación).</remarks>
    /// <param name="request">Número de página (1-based) y tamaño de página.</param>
    /// <response code="200">Página de resultados con metadatos</response>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] PagedRequest request, CancellationToken cancellationToken)
    {
        var result = await _productService.GetPagedAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>Obtiene un producto por su ID.</summary>
    /// <param name="id">Identificador único del producto (GUID).</param>
    /// <response code="200">Producto encontrado</response>
    /// <response code="404">Producto no encontrado</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        if (product is null)
            return NotFound(new { error = $"Product with ID '{id}' not found." });

        return Ok(product);
    }

    /// <summary>Obtiene un producto por su código único.</summary>
    /// <param name="code">Código único del producto (ej: LAP-001).</param>
    /// <response code="200">Producto encontrado</response>
    /// <response code="404">Producto no encontrado</response>
    [HttpGet("code/{code}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByCodeAsync(code, cancellationToken);
        if (product is null)
            return NotFound(new { error = $"Product with code '{code}' not found." });

        return Ok(product);
    }

    /// <summary>Crea un nuevo producto.</summary>
    /// <remarks>Registra un producto con código único, nombre, precio, stock y moneda.</remarks>
    /// <response code="201">Producto creado exitosamente</response>
    /// <response code="400">Datos inválidos o código duplicado</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto, CancellationToken cancellationToken)
    {
        var product = await _productService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>Actualiza un producto existente.</summary>
    /// <remarks>Modifica nombre, descripción, precio, stock, moneda y estado activo/inactivo.</remarks>
    /// <param name="id">Identificador único del producto (GUID).</param>
    /// <param name="dto">Datos actualizados del producto.</param>
    /// <response code="200">Producto actualizado</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="404">Producto no encontrado</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto, CancellationToken cancellationToken)
    {
        var product = await _productService.UpdateAsync(id, dto, cancellationToken);
        return Ok(product);
    }

    /// <summary>Elimina un producto del catálogo.</summary>
    /// <param name="id">Identificador único del producto (GUID).</param>
    /// <response code="204">Producto eliminado</response>
    /// <response code="404">Producto no encontrado</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _productService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>Agrega stock a un producto (reposición de inventario).</summary>
    /// <param name="id">Identificador único del producto (GUID).</param>
    /// <param name="request">Cantidad positiva de unidades a agregar.</param>
    /// <response code="200">Stock actualizado</response>
    /// <response code="400">Cantidad inválida</response>
    /// <response code="404">Producto no encontrado</response>
    [HttpPatch("{id:guid}/stock/add")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddStock(Guid id, [FromBody] StockRequest request, CancellationToken cancellationToken)
    {
        var product = await _productService.AddStockAsync(id, request.Quantity, cancellationToken);
        return Ok(product);
    }

    /// <summary>Remueve stock de un producto (venta, daño, ajuste).</summary>
    /// <param name="id">Identificador único del producto (GUID).</param>
    /// <param name="request">Cantidad positiva de unidades a remover.</param>
    /// <response code="200">Stock actualizado</response>
    /// <response code="400">Stock insuficiente o cantidad inválida</response>
    /// <response code="404">Producto no encontrado</response>
    [HttpPatch("{id:guid}/stock/remove")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveStock(Guid id, [FromBody] StockRequest request, CancellationToken cancellationToken)
    {
        var product = await _productService.RemoveStockAsync(id, request.Quantity, cancellationToken);
        return Ok(product);
    }
}
