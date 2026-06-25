using testNet.Domain.Entities;

namespace testNet.Application.DTOs;

public static class ProductMappingExtensions
{
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            Description = product.Description,
            Price = product.UnitPrice.Amount,
            Currency = product.UnitPrice.Currency,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
