using testNet.Domain.Exceptions;
using testNet.Domain.ValueObjects;

namespace testNet.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }

    public string CodeValue { get; private set; } = null!;
    public decimal PriceAmount { get; private set; }
    public string CurrencyCode { get; private set; } = "USD";

    public ProductCode Code
    {
        get => new(CodeValue);
        private set => CodeValue = value.Value;
    }

    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    public Price UnitPrice
    {
        get => new(PriceAmount, CurrencyCode);
        private set
        {
            PriceAmount = value.Amount;
            CurrencyCode = value.Currency;
        }
    }

    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Product() { }

    public Product(string code, string name, string description, decimal price, int stockQuantity, string currency = "USD")
    {
        Id = Guid.NewGuid();
        Code = new ProductCode(code);
        SetName(name);
        SetDescription(description);
        UnitPrice = new Price(price, currency);
        SetStock(stockQuantity);
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name cannot be empty.");
        if (name.Length > 200)
            throw new DomainException("Product name cannot exceed 200 characters.");

        Name = name;
        MarkUpdated();
    }

    public void SetDescription(string description)
    {
        Description = description ?? string.Empty;
        MarkUpdated();
    }

    public void UpdatePrice(decimal amount, string currency = "USD")
    {
        UnitPrice = new Price(amount, currency);
        MarkUpdated();
    }

    public void SetStock(int quantity)
    {
        if (quantity < 0)
            throw new DomainException("Stock quantity cannot be negative.");

        StockQuantity = quantity;
        MarkUpdated();
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity to add must be positive.");

        StockQuantity += quantity;
        MarkUpdated();
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity to remove must be positive.");

        if (StockQuantity < quantity)
            throw new DomainException($"Insufficient stock. Available: {StockQuantity}, Requested: {quantity}.");

        StockQuantity -= quantity;
        MarkUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkUpdated();
    }

    private void MarkUpdated() => UpdatedAt = DateTime.UtcNow;
}
