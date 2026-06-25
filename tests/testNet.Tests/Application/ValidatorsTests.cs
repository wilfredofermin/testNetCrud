using FluentAssertions;
using testNet.Application.DTOs;
using testNet.Application.Validators;
using testNet.Domain.Exceptions;

namespace testNet.Tests.Application;

public class ValidatorsTests
{
    [Fact]
    public void CreateProductValidator_WithValidDto_ShouldNotThrow()
    {
        var dto = new CreateProductDto
        {
            Code = "P001",
            Name = "Test",
            Price = 10,
            StockQuantity = 5
        };

        Action act = () => CreateProductValidator.Validate(dto);
        act.Should().NotThrow();
    }

    [Fact]
    public void CreateProductValidator_WithMissingCode_ShouldThrow()
    {
        var dto = new CreateProductDto { Code = "", Name = "Test", Price = 10, StockQuantity = 5 };

        Action act = () => CreateProductValidator.Validate(dto);
        act.Should().Throw<DomainException>().WithMessage("*Code*");
    }

    [Fact]
    public void CreateProductValidator_WithNegativePrice_ShouldThrow()
    {
        var dto = new CreateProductDto { Code = "P001", Name = "Test", Price = -1, StockQuantity = 5 };

        Action act = () => CreateProductValidator.Validate(dto);
        act.Should().Throw<DomainException>().WithMessage("*Price*");
    }
}
