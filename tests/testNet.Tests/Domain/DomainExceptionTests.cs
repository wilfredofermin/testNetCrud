using FluentAssertions;
using testNet.Domain.Exceptions;

namespace testNet.Tests.Domain;

public class DomainExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        var ex = new DomainException("something failed");

        ex.Message.Should().Be("something failed");
        ex.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithInnerException_ShouldSetInnerException()
    {
        var inner = new InvalidOperationException("inner");

        var ex = new DomainException("outer", inner);

        ex.Message.Should().Be("outer");
        ex.InnerException.Should().BeSameAs(inner);
    }
}