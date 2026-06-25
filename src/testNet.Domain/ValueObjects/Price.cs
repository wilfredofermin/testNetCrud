using testNet.Domain.Exceptions;

namespace testNet.Domain.ValueObjects;

public sealed record Price
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Price(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new DomainException("Price cannot be negative.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency cannot be empty.");

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public override string ToString() => $"{Amount:F2} {Currency}";
}
