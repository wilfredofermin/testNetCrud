using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using testNet.Domain.Entities;
using testNet.Infrastructure.Data;
using testNet.Infrastructure.Repositories;

namespace testNet.Tests.Infrastructure;

public class ProductRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"RepoTest_{Guid.NewGuid()}")
            .Options;
        return new AppDbContext(options);
    }

    private static Product CreateProduct(string code = "P001", string name = "Sample")
        => new(code, name, "Desc", 10m, 5);

    [Fact]
    public async Task AddAsync_ShouldPersistProduct()
    {
        await using var context = CreateContext();
        var repo = new ProductRepository(context);
        var product = CreateProduct();

        await repo.AddAsync(product);

        context.Products.Should().ContainSingle(p => p.Id == product.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingProduct_ShouldReturnProduct()
    {
        await using var context = CreateContext();
        var product = CreateProduct();
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var repo = new ProductRepository(context);
        var result = await repo.GetByIdAsync(product.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(product.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingProduct_ShouldReturnNull()
    {
        await using var context = CreateContext();
        var repo = new ProductRepository(context);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByCodeAsync_ExistingProduct_ShouldReturnProduct()
    {
        await using var context = CreateContext();
        var product = CreateProduct("CODE-X");
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var repo = new ProductRepository(context);
        var result = await repo.GetByCodeAsync("CODE-X");

        result.Should().NotBeNull();
        result!.CodeValue.Should().Be("CODE-X");
    }

    [Fact]
    public async Task GetByCodeAsync_NonExistingProduct_ShouldReturnNull()
    {
        await using var context = CreateContext();
        var repo = new ProductRepository(context);

        var result = await repo.GetByCodeAsync("NOPE");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPersistedProducts()
    {
        await using var context = CreateContext();
        context.Products.AddRange(CreateProduct("A1"), CreateProduct("A2"));
        await context.SaveChangesAsync();

        var repo = new ProductRepository(context);
        var result = await repo.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetActiveAsync_ShouldReturnOnlyActiveProducts()
    {
        await using var context = CreateContext();
        var active = CreateProduct("ACT");
        var inactive = CreateProduct("INA");
        inactive.Deactivate();
        context.Products.AddRange(active, inactive);
        await context.SaveChangesAsync();

        var repo = new ProductRepository(context);
        var result = await repo.GetActiveAsync();

        result.Should().ContainSingle(p => p.CodeValue == "ACT");
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnRequestedPageAndTotalCount()
    {
        await using var context = CreateContext();
        for (var i = 0; i < 15; i++)
            context.Products.Add(CreateProduct($"P{i:D2}"));
        await context.SaveChangesAsync();

        var repo = new ProductRepository(context);
        var (items, totalCount) = await repo.GetPagedAsync(2, 10);

        totalCount.Should().Be(15);
        items.Should().HaveCount(5);
    }

    [Fact]
    public async Task ExistsByCodeAsync_WhenProductExists_ShouldReturnTrue()
    {
        await using var context = CreateContext();
        context.Products.Add(CreateProduct("EXISTS"));
        await context.SaveChangesAsync();

        var repo = new ProductRepository(context);
        var exists = await repo.ExistsByCodeAsync("EXISTS");

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByCodeAsync_WhenProductDoesNotExist_ShouldReturnFalse()
    {
        await using var context = CreateContext();
        var repo = new ProductRepository(context);

        var exists = await repo.ExistsByCodeAsync("MISSING");

        exists.Should().BeFalse();
    }

    [Fact]
    public async Task Update_ShouldMarkProductAsModified()
    {
        await using var context = CreateContext();
        var product = CreateProduct("UPD");
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var repo = new ProductRepository(context);
        product.SetName("Updated Name");
        repo.Update(product);

        context.Products.Single(p => p.Id == product.Id).Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task Delete_ShouldRemoveProduct()
    {
        await using var context = CreateContext();
        var product = CreateProduct("DEL");
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var repo = new ProductRepository(context);
        repo.Delete(product);

        context.Products.Should().BeEmpty();
    }
}