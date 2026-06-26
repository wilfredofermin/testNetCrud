using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using testNet.Infrastructure.Data;

namespace testNet.Tests.Infrastructure;

public class DbSeederTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"SeederTest_{Guid.NewGuid()}")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task SeedAsync_WhenEmpty_ShouldSeedAllProducts()
    {
        await using var context = CreateContext();

        await DbSeeder.SeedAsync(context);

        context.Products.Should().HaveCount(10);
    }

    [Fact]
    public async Task SeedAsync_WhenAlreadySeeded_ShouldNotSeedAgain()
    {
        await using var context = CreateContext();
        context.Products.Add(new("X1", "X", "", 1, 1));
        await context.SaveChangesAsync();

        await DbSeeder.SeedAsync(context);

        context.Products.Should().ContainSingle();
    }

    [Fact]
    public async Task ResetAndSeedAsync_ShouldClearAndReseedDatabase()
    {
        await using var context = CreateContext();
        context.Products.Add(new("OLD", "Old", "", 1, 1));
        await context.SaveChangesAsync();

        var count = await DbSeeder.ResetAndSeedAsync(context);

        count.Should().Be(10);
        context.Products.Should().HaveCount(10);
        context.Products.Any(p => p.CodeValue == "OLD").Should().BeFalse();
    }
}