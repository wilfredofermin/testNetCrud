using testNet.Application.Common;
using testNet.Application.DTOs;

namespace testNet.Application.Interfaces;

public interface IProductService
{
    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductDto>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<ProductDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default);
    Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductDto> AddStockAsync(Guid id, int quantity, CancellationToken cancellationToken = default);
    Task<ProductDto> RemoveStockAsync(Guid id, int quantity, CancellationToken cancellationToken = default);
}
