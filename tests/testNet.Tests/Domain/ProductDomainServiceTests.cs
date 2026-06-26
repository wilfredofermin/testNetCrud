using FluentAssertions;
using Moq;
using testNet.Domain.DomainServices;
using testNet.Domain.Entities;
using testNet.Domain.Exceptions;
using testNet.Domain.Interfaces;

namespace testNet.Tests.Domain;

public class ProductDomainServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly ProductDomainService _domainService;

    public ProductDomainServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _domainService = new ProductDomainService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateProductAsync_WithUniqueCode_ShouldReturnProduct()
    {
        _repositoryMock.Setup(r => r.ExistsByCodeAsync("P001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var product = await _domainService.CreateProductAsync("P001", "Test", "Desc", 10, 100);

        product.Should().NotBeNull();
        product.Code.Value.Should().Be("P001");
    }

    [Fact]
    public async Task CreateProductAsync_WithDuplicateCode_ShouldThrow()
    {
        _repositoryMock.Setup(r => r.ExistsByCodeAsync("P001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Func<Task> act = () => _domainService.CreateProductAsync("P001", "Test", "Desc", 10, 100);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task TransferStockAsync_WithValidData_ShouldTransfer()
    {
        var fromProduct = new Product("SRC", "Source", "", 10, 100);
        var toProduct = new Product("DST", "Dest", "", 10, 50);

        _repositoryMock.Setup(r => r.GetByIdAsync(fromProduct.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fromProduct);
        _repositoryMock.Setup(r => r.GetByIdAsync(toProduct.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(toProduct);

        var result = await _domainService.TransferStockAsync(fromProduct.Id, toProduct.Id, 30);

        result.StockQuantity.Should().Be(80);
        fromProduct.StockQuantity.Should().Be(70);
    }

    [Fact]
    public async Task TransferStockAsync_WithInsufficientStock_ShouldThrow()
    {
        var fromProduct = new Product("SRC", "Source", "", 10, 10);
        var toProduct = new Product("DST", "Dest", "", 10, 50);

        _repositoryMock.Setup(r => r.GetByIdAsync(fromProduct.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fromProduct);
        _repositoryMock.Setup(r => r.GetByIdAsync(toProduct.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(toProduct);

        Func<Task> act = () => _domainService.TransferStockAsync(fromProduct.Id, toProduct.Id, 30);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*Insufficient stock*");
    }

    [Fact]
    public async Task TransferStockAsync_WhenSourceNotFound_ShouldThrow()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        Func<Task> act = () => _domainService.TransferStockAsync(Guid.NewGuid(), Guid.NewGuid(), 5);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Source product*");
    }

    [Fact]
    public async Task TransferStockAsync_WhenDestinationNotFound_ShouldThrow()
    {
        var fromProduct = new Product("SRC", "Source", "", 10, 100);
        _repositoryMock.Setup(r => r.GetByIdAsync(fromProduct.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fromProduct);
        _repositoryMock.Setup(r => r.GetByIdAsync(It.Is<Guid>(g => g != fromProduct.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        Func<Task> act = () => _domainService.TransferStockAsync(fromProduct.Id, Guid.NewGuid(), 5);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Destination product*");
    }
}
