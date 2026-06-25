using testNet.Domain.Exceptions;

namespace testNet.Domain.ValueObjects;

public sealed record ProductCode
{
    public string Value { get; }

    public ProductCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Product code cannot be empty.");

        if (value.Length > 50)
            throw new DomainException("Product code cannot exceed 50 characters.");

        Value = value;
    }

    public override string ToString() => Value;

    public static implicit operator string(ProductCode code) => code.Value;

    public static implicit operator ProductCode(string value) => new(value);
}
