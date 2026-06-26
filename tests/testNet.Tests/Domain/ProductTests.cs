using FluentAssertions;
using testNet.Domain.Entities;
using testNet.Domain.Exceptions;

namespace testNet.Tests.Domain;

public class ProductTests
{
    [Fact]
    public void CreateProduct_WithValidData_ShouldSucceed()
    {
        var product = new Product("P001", "Test Product", "Description", 10.99m, 100);

        product.Id.Should().NotBeEmpty();
        product.Name.Should().Be("Test Product");
        product.Code.Value.Should().Be("P001");
        product.UnitPrice.Amount.Should().Be(10.99m);
        product.StockQuantity.Should().Be(100);
        product.IsActive.Should().BeTrue();
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("", "Name", "Desc", 10, 10)]
    [InlineData("CODE", "", "Desc", 10, 10)]
    [InlineData("CODE", "Name", "Desc", -1, 10)]
    [InlineData("CODE", "Name", "Desc", 10, -1)]
    public void CreateProduct_WithInvalidData_ShouldThrow(string code, string name, string desc, decimal price, int stock)
    {
        Action act = () => new Product(code, name, desc, price, stock);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void SetName_WithValidValue_ShouldUpdate()
    {
        var product = CreateValidProduct();
        product.SetName("New Name");

        product.Name.Should().Be("New Name");
        product.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SetName_WithEmptyValue_ShouldThrow()
    {
        var product = CreateValidProduct();

        Action act = () => product.SetName("");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void UpdatePrice_WithValidValue_ShouldUpdate()
    {
        var product = CreateValidProduct();
        product.UpdatePrice(25.50m, "EUR");

        product.UnitPrice.Amount.Should().Be(25.50m);
        product.UnitPrice.Currency.Should().Be("EUR");
    }

    [Fact]
    public void UpdatePrice_WithNegativeValue_ShouldThrow()
    {
        var product = CreateValidProduct();

        Action act = () => product.UpdatePrice(-1);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddStock_WithPositiveQuantity_ShouldIncrease()
    {
        var product = CreateValidProduct();
        product.AddStock(50);

        product.StockQuantity.Should().Be(150);
    }

    [Fact]
    public void AddStock_WithNonPositiveQuantity_ShouldThrow()
    {
        var product = CreateValidProduct();

        Action act = () => product.AddStock(0);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void RemoveStock_WithSufficientQuantity_ShouldDecrease()
    {
        var product = CreateValidProduct();
        product.RemoveStock(30);

        product.StockQuantity.Should().Be(70);
    }

    [Fact]
    public void RemoveStock_WithInsufficientQuantity_ShouldThrow()
    {
        var product = CreateValidProduct();

        Action act = () => product.RemoveStock(200);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void RemoveStock_WithNonPositiveQuantity_ShouldThrow()
    {
        var product = CreateValidProduct();

        Action act = () => product.RemoveStock(-5);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Activate_ShouldSetIsActiveTrue()
    {
        var product = CreateValidProduct();
        product.Deactivate();
        product.Activate();

        product.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var product = CreateValidProduct();
        product.Deactivate();

        product.IsActive.Should().BeFalse();
    }

    [Fact]
    public void SetName_WithTooLongValue_ShouldThrow()
    {
        var product = CreateValidProduct();
        var longName = new string('A', 201);

        Action act = () => product.SetName(longName);

        act.Should().Throw<DomainException>().WithMessage("*200 characters*");
    }

    [Fact]
    public void SetDescription_WithNull_ShouldSetEmptyString()
    {
        var product = CreateValidProduct();

        product.SetDescription(null!);

        product.Description.Should().BeEmpty();
    }

    [Fact]
    public void SetStock_WithNegativeValue_ShouldThrow()
    {
        var product = CreateValidProduct();

        Action act = () => product.SetStock(-1);

        act.Should().Throw<DomainException>().WithMessage("*negative*");
    }

    private static Product CreateValidProduct()
    {
        return new Product("P001", "Test Product", "Description", 10.99m, 100);
    }
}
