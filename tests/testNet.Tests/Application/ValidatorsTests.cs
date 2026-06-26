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

    [Fact]
    public void CreateProductValidator_WithMissingName_ShouldThrow()
    {
        var dto = new CreateProductDto { Code = "P001", Name = "", Price = 10, StockQuantity = 5 };

        Action act = () => CreateProductValidator.Validate(dto);
        act.Should().Throw<DomainException>().WithMessage("*Name*");
    }

    [Fact]
    public void CreateProductValidator_WithNegativeStock_ShouldThrow()
    {
        var dto = new CreateProductDto { Code = "P001", Name = "Test", Price = 10, StockQuantity = -1 };

        Action act = () => CreateProductValidator.Validate(dto);
        act.Should().Throw<DomainException>().WithMessage("*Stock*");
    }

    [Fact]
    public void CreateProductValidator_WithMultipleErrors_ShouldAggregateAllErrors()
    {
        var dto = new CreateProductDto { Code = "", Name = "", Price = -1, StockQuantity = -1 };

        Action act = () => CreateProductValidator.Validate(dto);
        var thrown = act.Should().Throw<DomainException>().Which;
        thrown.Message.Should().Contain("Code");
        thrown.Message.Should().Contain("Name");
        thrown.Message.Should().Contain("Price");
        thrown.Message.Should().Contain("Stock");
    }

    [Fact]
    public void UpdateProductValidator_WithValidDto_ShouldNotThrow()
    {
        var dto = new UpdateProductDto
        {
            Name = "Updated",
            Description = "Desc",
            Price = 20,
            StockQuantity = 10
        };

        Action act = () => UpdateProductValidator.Validate(dto);
        act.Should().NotThrow();
    }

    [Fact]
    public void UpdateProductValidator_WithMissingName_ShouldThrow()
    {
        var dto = new UpdateProductDto { Name = "", Price = 10, StockQuantity = 5 };

        Action act = () => UpdateProductValidator.Validate(dto);
        act.Should().Throw<DomainException>().WithMessage("*Name*");
    }

    [Fact]
    public void UpdateProductValidator_WithNegativePrice_ShouldThrow()
    {
        var dto = new UpdateProductDto { Name = "Test", Price = -1, StockQuantity = 5 };

        Action act = () => UpdateProductValidator.Validate(dto);
        act.Should().Throw<DomainException>().WithMessage("*Price*");
    }

    [Fact]
    public void UpdateProductValidator_WithNegativeStock_ShouldThrow()
    {
        var dto = new UpdateProductDto { Name = "Test", Price = 10, StockQuantity = -1 };

        Action act = () => UpdateProductValidator.Validate(dto);
        act.Should().Throw<DomainException>().WithMessage("*Stock*");
    }

    [Fact]
    public void UpdateProductValidator_WithMultipleErrors_ShouldAggregateAllErrors()
    {
        var dto = new UpdateProductDto { Name = "", Price = -1, StockQuantity = -1 };

        Action act = () => UpdateProductValidator.Validate(dto);
        var thrown = act.Should().Throw<DomainException>().Which;
        thrown.Message.Should().Contain("Name");
        thrown.Message.Should().Contain("Price");
        thrown.Message.Should().Contain("Stock");
    }
}
