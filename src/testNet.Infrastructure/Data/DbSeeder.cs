using testNet.Domain.Entities;

namespace testNet.Infrastructure.Data;

public static class DbSeeder
{
    private static readonly List<Product> SeedProducts =
    [
        new("LAP-001", "Laptop Gamer RTX 4080", "Laptop de alta gama con NVIDIA RTX 4080, 32GB RAM, 1TB SSD", 2499.99m, 15),
        new("LAP-002", "Laptop Office Pro", "Laptop empresarial i7, 16GB RAM, 512GB SSD", 1299.99m, 30),
        new("MOU-001", "Mouse RGB Gaming", "Mouse ergonómico con 8 botones programables, RGB personalizable", 79.99m, 100),
        new("MOU-002", "Mouse Bluetooth Slim", "Mouse ultra delgado, batería recargable, conexión Bluetooth 5.0", 39.99m, 200),
        new("TEC-001", "Teclado Mecánico RGB", "Teclado mecánico switches Cherry MX, retroiluminación RGB", 149.99m, 50),
        new("MON-001", "Monitor 27\" 4K IPS", "Monitor UHD 4K, panel IPS, HDR400, 60Hz", 449.99m, 25),
        new("MON-002", "Monitor 24\" Full HD", "Monitor FHD 144Hz, 1ms, ideal para gaming competitivo", 249.99m, 40),
        new("AUD-001", "Audífonos Inalámbricos", "Audífonos over-ear con cancelación activa de ruido, 30h batería", 199.99m, 60),
        new("WEB-001", "Webcam 4K Pro", "Cámara web 4K con micrófono estéreo, enfoque automático", 129.99m, 35),
        new("DIS-001", "SSD NVMe 1TB", "Disco sólido NVMe M.2, lectura 7000MB/s, escritura 5000MB/s", 119.99m, 80),
    ];

    public static async Task SeedAsync(AppDbContext context)
    {
        if (context.Products.Any())
            return;

        await SeedInternalAsync(context);
    }

    public static async Task<int> ResetAndSeedAsync(AppDbContext context)
    {
        context.Products.RemoveRange(context.Products);
        await context.SaveChangesAsync();

        await SeedInternalAsync(context);

        return SeedProducts.Count;
    }

    private static async Task SeedInternalAsync(AppDbContext context)
    {
        context.Products.AddRange(SeedProducts);
        await context.SaveChangesAsync();
    }
}
