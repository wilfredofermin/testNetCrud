using FluentAssertions;
using testNet.Application.DTOs;
using testNet.Domain.Entities;

namespace testNet.Tests.Application;

public class ProductMappingTests
{
    [Fact]
    public void ToDto_ShouldMapAllProperties()
    {
        var product = new Product("P001", "Test Product", "Description", 15.99m, 100);

        var dto = product.ToDto();

        dto.Id.Should().Be(product.Id);
        dto.Code.Should().Be(product.Code.Value);
        dto.Name.Should().Be(product.Name);
        dto.Description.Should().Be(product.Description);
        dto.Price.Should().Be(product.UnitPrice.Amount);
        dto.Currency.Should().Be(product.UnitPrice.Currency);
        dto.StockQuantity.Should().Be(product.StockQuantity);
        dto.IsActive.Should().Be(product.IsActive);
        dto.CreatedAt.Should().Be(product.CreatedAt);
    }
}
