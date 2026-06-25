using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using testNet.API.Controllers;
using testNet.Application.Common;
using testNet.Application.DTOs;
using testNet.Application.Interfaces;

namespace testNet.Tests.API;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _serviceMock;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _serviceMock = new Mock<IProductService>();
        _controller = new ProductsController(_serviceMock.Object);
    }

    private static ProductDto CreateSampleDto()
    {
        return new ProductDto
        {
            Id = Guid.NewGuid(),
            Code = "P001",
            Name = "Sample",
            Description = "Desc",
            Price = 9.99m,
            Currency = "USD",
            StockQuantity = 50,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithProducts()
    {
        var products = new[] { CreateSampleDto() };
        _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var result = await _controller.GetAll(CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeAssignableTo<IEnumerable<ProductDto>>();
    }

    [Fact]
    public async Task GetActive_ShouldReturnOk()
    {
        _serviceMock.Setup(s => s.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _controller.GetActive(CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetPaged_ShouldReturnOkWithPagedResult()
    {
        var paged = new PagedResult<ProductDto>
        {
            Items = [CreateSampleDto()],
            Page = 1,
            PageSize = 10,
            TotalCount = 1
        };
        _serviceMock.Setup(s => s.GetPagedAsync(It.IsAny<PagedRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _controller.GetPaged(new PagedRequest(), CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<PagedResult<ProductDto>>();
    }

    [Fact]
    public async Task GetById_ExistingProduct_ShouldReturnOk()
    {
        var dto = CreateSampleDto();
        _serviceMock.Setup(s => s.GetByIdAsync(dto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _controller.GetById(dto.Id, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<ProductDto>();
    }

    [Fact]
    public async Task GetById_NonExistingProduct_ShouldReturnNotFound()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductDto?)null);

        var result = await _controller.GetById(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetByCode_ExistingProduct_ShouldReturnOk()
    {
        var dto = CreateSampleDto();
        _serviceMock.Setup(s => s.GetByCodeAsync("P001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _controller.GetByCode("P001", CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByCode_NonExistingProduct_ShouldReturnNotFound()
    {
        _serviceMock.Setup(s => s.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductDto?)null);

        var result = await _controller.GetByCode("FAKE", CancellationToken.None);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Create_WithValidDto_ShouldReturnCreated()
    {
        var dto = CreateSampleDto();
        _serviceMock.Setup(s => s.CreateAsync(It.IsAny<CreateProductDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _controller.Create(new CreateProductDto(), CancellationToken.None);

        result.Should().BeOfType<CreatedAtActionResult>();
        var created = result as CreatedAtActionResult;
        created!.RouteValues!["id"].Should().Be(dto.Id);
    }

    [Fact]
    public async Task Update_WithValidData_ShouldReturnOk()
    {
        var dto = CreateSampleDto();
        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateProductDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _controller.Update(Guid.NewGuid(), new UpdateProductDto(), CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<ProductDto>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent()
    {
        _serviceMock.Setup(s => s.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Delete(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task AddStock_ShouldReturnOk()
    {
        var dto = CreateSampleDto();
        _serviceMock.Setup(s => s.AddStockAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _controller.AddStock(Guid.NewGuid(), new StockRequest { Quantity = 10 }, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task RemoveStock_ShouldReturnOk()
    {
        var dto = CreateSampleDto();
        _serviceMock.Setup(s => s.RemoveStockAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _controller.RemoveStock(Guid.NewGuid(), new StockRequest { Quantity = 5 }, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
    }
}
