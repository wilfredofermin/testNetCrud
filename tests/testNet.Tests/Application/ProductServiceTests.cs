using FluentAssertions;
using Moq;
using testNet.Application.Common;
using testNet.Application.DTOs;
using testNet.Application.Services;
using testNet.Domain.DomainServices;
using testNet.Domain.Entities;
using testNet.Domain.Exceptions;
using testNet.Domain.Interfaces;

namespace testNet.Tests.Application;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly ProductDomainService _domainService;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _domainService = new ProductDomainService(_repositoryMock.Object);
        _productService = new ProductService(_repositoryMock.Object, _domainService);
    }

    private static Product CreateSampleProduct()
    {
        return new Product("P001", "Sample", "Desc", 9.99m, 50);
    }

    private static CreateProductDto CreateSampleCreateDto()
    {
        return new CreateProductDto
        {
            Code = "P001",
            Name = "Sample",
            Description = "Desc",
            Price = 9.99m,
            StockQuantity = 50,
            Currency = "USD"
        };
    }

    private static UpdateProductDto CreateSampleUpdateDto()
    {
        return new UpdateProductDto
        {
            Name = "Updated",
            Description = "Updated Desc",
            Price = 19.99m,
            StockQuantity = 100,
            Currency = "USD",
            IsActive = true
        };
    }

    [Fact]
    public async Task GetByIdAsync_ExistingProduct_ShouldReturnDto()
    {
        var product = CreateSampleProduct();
        _repositoryMock.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _productService.GetByIdAsync(product.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(product.Id);
        result.Name.Should().Be("Sample");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingProduct_ShouldReturnNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var result = await _productService.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByCodeAsync_ExistingProduct_ShouldReturnDto()
    {
        var product = CreateSampleProduct();
        _repositoryMock.Setup(r => r.GetByCodeAsync("P001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _productService.GetByCodeAsync("P001");

        result.Should().NotBeNull();
        result!.Code.Should().Be("P001");
    }

    [Fact]
    public async Task GetByCodeAsync_NonExistingProduct_ShouldReturnNull()
    {
        _repositoryMock.Setup(r => r.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var result = await _productService.GetByCodeAsync("MISSING");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProducts()
    {
        var products = new List<Product>
        {
            new("P001", "A", "", 10, 10),
            new("P002", "B", "", 20, 20)
        };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var result = await _productService.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetActiveAsync_ShouldReturnOnlyActive()
    {
        var active = new Product("P001", "Active", "", 10, 10);
        _repositoryMock.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { active });

        var result = await _productService.GetActiveAsync();

        result.Should().ContainSingle();
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnPagedResult()
    {
        var products = new[] { CreateSampleProduct() };
        _repositoryMock.Setup(r => r.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 1));

        var result = await _productService.GetPagedAsync(new PagedRequest { Page = 1, PageSize = 10 });

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
        result.Page.Should().Be(1);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_WithValidDto_ShouldCreateProduct()
    {
        _repositoryMock.Setup(r => r.ExistsByCodeAsync("P001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var dto = CreateSampleCreateDto();
        var result = await _productService.CreateAsync(dto);

        result.Should().NotBeNull();
        result.Name.Should().Be("Sample");
        result.Code.Should().Be("P001");
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateCode_ShouldThrow()
    {
        _repositoryMock.Setup(r => r.ExistsByCodeAsync("P001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var dto = CreateSampleCreateDto();

        Func<Task> act = () => _productService.CreateAsync(dto);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task UpdateAsync_ExistingProduct_ShouldUpdate()
    {
        var product = CreateSampleProduct();
        _repositoryMock.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var dto = CreateSampleUpdateDto();
        var result = await _productService.UpdateAsync(product.Id, dto);

        result.Should().NotBeNull();
        result.Name.Should().Be("Updated");
        result.Price.Should().Be(19.99m);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingProduct_ShouldThrow()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var dto = CreateSampleUpdateDto();

        Func<Task> act = () => _productService.UpdateAsync(Guid.NewGuid(), dto);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task UpdateAsync_WithInactiveFlag_ShouldDeactivateProduct()
    {
        var product = CreateSampleProduct();
        _repositoryMock.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var dto = new UpdateProductDto
        {
            Name = "Updated",
            Description = "Desc",
            Price = 19.99m,
            StockQuantity = 100,
            IsActive = false
        };
        var result = await _productService.UpdateAsync(product.Id, dto);

        result.IsActive.Should().BeFalse();
        _repositoryMock.Verify(r => r.Update(product), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ExistingProduct_ShouldDelete()
    {
        var product = CreateSampleProduct();
        _repositoryMock.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        await _productService.DeleteAsync(product.Id);

        _repositoryMock.Verify(r => r.Delete(product), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingProduct_ShouldThrow()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        Func<Task> act = () => _productService.DeleteAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task AddStockAsync_ShouldIncreaseStock()
    {
        var product = CreateSampleProduct();
        _repositoryMock.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _productService.AddStockAsync(product.Id, 30);

        result.StockQuantity.Should().Be(80);
    }

    [Fact]
    public async Task RemoveStockAsync_WithSufficientStock_ShouldDecrease()
    {
        var product = CreateSampleProduct();
        _repositoryMock.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _productService.RemoveStockAsync(product.Id, 20);

        result.StockQuantity.Should().Be(30);
    }

    [Fact]
    public async Task RemoveStockAsync_WithInsufficientStock_ShouldThrow()
    {
        var product = CreateSampleProduct();
        _repositoryMock.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        Func<Task> act = () => _productService.RemoveStockAsync(product.Id, 999);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task AddStockAsync_NonExistingProduct_ShouldThrow()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        Func<Task> act = () => _productService.AddStockAsync(Guid.NewGuid(), 10);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*not found*");
    }

    [Fact]
    public async Task RemoveStockAsync_NonExistingProduct_ShouldThrow()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        Func<Task> act = () => _productService.RemoveStockAsync(Guid.NewGuid(), 10);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*not found*");
    }
}
