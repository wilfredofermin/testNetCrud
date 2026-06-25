using testNet.Domain.Entities;
using testNet.Domain.Exceptions;
using testNet.Domain.Interfaces;

namespace testNet.Domain.DomainServices;

public class ProductDomainService
{
    private readonly IProductRepository _repository;

    public ProductDomainService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Product> CreateProductAsync(string code, string name, string description, decimal price, int stock, string currency = "USD")
    {
        if (await _repository.ExistsByCodeAsync(code))
            throw new DomainException($"A product with code '{code}' already exists.");

        return new Product(code, name, description, price, stock, currency);
    }

    public async Task<Product> TransferStockAsync(Guid fromProductId, Guid toProductId, int quantity)
    {
        var fromProduct = await _repository.GetByIdAsync(fromProductId)
            ?? throw new DomainException($"Source product with ID '{fromProductId}' not found.");

        var toProduct = await _repository.GetByIdAsync(toProductId)
            ?? throw new DomainException($"Destination product with ID '{toProductId}' not found.");

        fromProduct.RemoveStock(quantity);
        toProduct.AddStock(quantity);

        _repository.Update(fromProduct);
        _repository.Update(toProduct);

        return toProduct;
    }
}
