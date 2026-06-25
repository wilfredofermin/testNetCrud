using FluentAssertions;
using testNet.Domain.Exceptions;
using testNet.Domain.ValueObjects;

namespace testNet.Tests.Domain;

public class ProductCodeTests
{
    [Fact]
    public void CreateProductCode_WithValidValue_ShouldSucceed()
    {
        var code = new ProductCode("PRD-001");
        code.Value.Should().Be("PRD-001");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CreateProductCode_WithEmptyValue_ShouldThrow(string? value)
    {
        Action act = () => new ProductCode(value!);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CreateProductCode_WithExceedingLength_ShouldThrow()
    {
        var longCode = new string('A', 51);
        Action act = () => new ProductCode(longCode);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        var code = new ProductCode("TEST");
        string value = code;
        value.Should().Be("TEST");
    }

    [Fact]
    public void ImplicitConversion_FromString_ShouldWork()
    {
        ProductCode code = "TEST";
        code.Value.Should().Be("TEST");
    }
}
