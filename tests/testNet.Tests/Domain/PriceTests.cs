using FluentAssertions;
using testNet.Domain.Exceptions;
using testNet.Domain.ValueObjects;

namespace testNet.Tests.Domain;

public class PriceTests
{
    [Fact]
    public void CreatePrice_WithValidValues_ShouldSucceed()
    {
        var price = new Price(10.99m, "USD");
        price.Amount.Should().Be(10.99m);
        price.Currency.Should().Be("USD");
    }

    [Fact]
    public void CreatePrice_WithDefaultCurrency_ShouldUseUSD()
    {
        var price = new Price(5.00m);
        price.Currency.Should().Be("USD");
    }

    [Fact]
    public void CreatePrice_NegativeAmount_ShouldThrow()
    {
        Action act = () => new Price(-1);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CreatePrice_EmptyCurrency_ShouldThrow()
    {
        Action act = () => new Price(10, "");
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Currency_ShouldBeUppercased()
    {
        var price = new Price(10, "eur");
        price.Currency.Should().Be("EUR");
    }

    [Fact]
    public void ToString_ShouldFormatCorrectly()
    {
        var price = new Price(10.50m, "USD");
        price.ToString().Should().Be("10.50 USD");
    }
}
