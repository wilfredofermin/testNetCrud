using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using testNet.Domain.DomainServices;
using testNet.Domain.Interfaces;
using testNet.Infrastructure.Data;
using testNet.Infrastructure.Repositories;

namespace testNet.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("TestNetDb"));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ProductDomainService>();

        return services;
    }
}
