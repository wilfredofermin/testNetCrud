using testNet.Application.DTOs;
using testNet.Domain.Exceptions;

namespace testNet.Application.Validators;

public static class CreateProductValidator
{
    public static void Validate(CreateProductDto dto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.Code))
            errors.Add("Code is required.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            errors.Add("Name is required.");

        if (dto.Price < 0)
            errors.Add("Price cannot be negative.");

        if (dto.StockQuantity < 0)
            errors.Add("Stock quantity cannot be negative.");

        if (errors.Count != 0)
            throw new DomainException($"Validation failed: {string.Join(" | ", errors)}");
    }
}
