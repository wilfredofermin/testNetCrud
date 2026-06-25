using testNet.Application.Common;
using testNet.Application.DTOs;
using testNet.Application.Interfaces;
using testNet.Application.Validators;
using testNet.Domain.DomainServices;
using testNet.Domain.Exceptions;
using testNet.Domain.Interfaces;

namespace testNet.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ProductDomainService _domainService;

    public ProductService(IProductRepository repository, ProductDomainService domainService)
    {
        _repository = repository;
        _domainService = domainService;
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken);
        return product?.ToDto();
    }

    public async Task<ProductDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByCodeAsync(code, cancellationToken);
        return product?.ToDto();
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _repository.GetAllAsync(cancellationToken);
        return products.Select(p => p.ToDto());
    }

    public async Task<IEnumerable<ProductDto>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var products = await _repository.GetActiveAsync(cancellationToken);
        return products.Select(p => p.ToDto());
    }

    public async Task<PagedResult<ProductDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(request.Page, request.PageSize, cancellationToken);

        return new PagedResult<ProductDto>
        {
            Items = items.Select(p => p.ToDto()),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        CreateProductValidator.Validate(dto);

        var product = await _domainService.CreateProductAsync(
            dto.Code, dto.Name, dto.Description, dto.Price, dto.StockQuantity, dto.Currency);

        await _repository.AddAsync(product, cancellationToken);
        return product.ToDto();
    }

    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        UpdateProductValidator.Validate(dto);

        var product = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new DomainException($"Product with ID '{id}' not found.");

        product.SetName(dto.Name);
        product.SetDescription(dto.Description);
        product.UpdatePrice(dto.Price, dto.Currency);
        product.SetStock(dto.StockQuantity);

        if (dto.IsActive)
            product.Activate();
        else
            product.Deactivate();

        _repository.Update(product);
        return product.ToDto();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new DomainException($"Product with ID '{id}' not found.");

        _repository.Delete(product);
    }

    public async Task<ProductDto> AddStockAsync(Guid id, int quantity, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new DomainException($"Product with ID '{id}' not found.");

        product.AddStock(quantity);
        _repository.Update(product);
        return product.ToDto();
    }

    public async Task<ProductDto> RemoveStockAsync(Guid id, int quantity, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new DomainException($"Product with ID '{id}' not found.");

        product.RemoveStock(quantity);
        _repository.Update(product);
        return product.ToDto();
    }
}
