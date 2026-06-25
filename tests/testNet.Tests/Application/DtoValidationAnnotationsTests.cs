using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using testNet.Application.Common;
using testNet.Application.DTOs;

namespace testNet.Tests.Application;

public class DtoValidationAnnotationsTests
{
    private static bool TryValidate(object dto, out List<ValidationResult> errors)
    {
        errors = [];
        var context = new ValidationContext(dto);
        return Validator.TryValidateObject(dto, context, errors, validateAllProperties: true);
    }

    public class CreateProductDtoAnnotations
    {
        [Fact]
        public void ValidDto_ShouldPassValidation()
        {
            var dto = new CreateProductDto
            {
                Code = "P001",
                Name = "Test",
                Description = "Desc",
                Price = 10,
                StockQuantity = 5,
                Currency = "USD"
            };

            var isValid = TryValidate(dto, out var errors);

            isValid.Should().BeTrue();
            errors.Should().BeEmpty();
        }

        [Fact]
        public void EmptyCode_ShouldFailValidation()
        {
            var dto = new CreateProductDto { Code = "", Name = "Test", Price = 10, StockQuantity = 5 };

            var isValid = TryValidate(dto, out var errors);

            isValid.Should().BeFalse();
            errors.Should().Contain(e => e.MemberNames.Contains("Code"));
        }

        [Fact]
        public void CodeExceedsMaxLength_ShouldFailValidation()
        {
            var dto = new CreateProductDto { Code = new('X', 51), Name = "Test", Price = 10, StockQuantity = 5 };

            var isValid = TryValidate(dto, out var errors);

            isValid.Should().BeFalse();
            errors.Should().Contain(e => e.MemberNames.Contains("Code"));
        }

        [Fact]
        public void EmptyName_ShouldFailValidation()
        {
            var dto = new CreateProductDto { Code = "P001", Name = "", Price = 10, StockQuantity = 5 };

            var isValid = TryValidate(dto, out var errors);

            isValid.Should().BeFalse();
            errors.Should().Contain(e => e.MemberNames.Contains("Name"));
        }

        [Fact]
        public void NameExceedsMaxLength_ShouldFailValidation()
        {
            var dto = new CreateProductDto { Code = "P001", Name = new('X', 201), Price = 10, StockQuantity = 5 };

            var isValid = TryValidate(dto, out var errors);

            isValid.Should().BeFalse();
            errors.Should().Contain(e => e.MemberNames.Contains("Name"));
        }

        [Fact]
        public void DescriptionExceedsMaxLength_ShouldFailValidation()
        {
            var dto = new CreateProductDto { Code = "P001", Name = "Test", Description = new('X', 2001), Price = 10, StockQuantity = 5 };

            var isValid = TryValidate(dto, out var errors);

            isValid.Should().BeFalse();
            errors.Should().Contain(e => e.MemberNames.Contains("Description"));
        }

        [Fact]
        public void NegativePrice_ShouldFailValidation()
        {
            var dto = new CreateProductDto { Code = "P001", Name = "Test", Price = -1, StockQuantity = 5 };

            var isValid = TryValidate(dto, out var errors);

            isValid.Should().BeFalse();
            errors.Should().Contain(e => e.MemberNames.Contains("Price"));
        }

        [Fact]
        public void NegativeStockQuantity_ShouldFailValidation()
        {
            var dto = new CreateProductDto { Code = "P001", Name = "Test", Price = 10, StockQuantity = -1 };

            var isValid = TryValidate(dto, out var errors);

            isValid.Should().BeFalse();
            errors.Should().Contain(e => e.MemberNames.Contains("StockQuantity"));
        }

        [Fact]
        public void InvalidCurrencyLength_ShouldFailValidation()
        {
            var dto = new CreateProductDto { Code = "P001", Name = "Test", Price = 10, StockQuantity = 5, Currency = "US" };

            var isValid = TryValidate(dto, out var errors);

            isValid.Should().BeFalse();
            errors.Should().Contain(e => e.MemberNames.Contains("Currency"));
        }
    }

    public class UpdateProductDtoAnnotations
    {
        [Fact]
        public void ValidDto_ShouldPassValidation()
        {
            var dto = new UpdateProductDto { Name = "Test", Price = 10, StockQuantity = 5, Currency = "USD", IsActive = true };

            var isValid = TryValidate(dto, out var errors);

            isValid.Should().BeTrue();
            errors.Should().BeEmpty();
        }

        [Fact]
        public void EmptyName_ShouldFailValidation()
        {
            var dto = new UpdateProductDto { Name = "", Price = 10, StockQuantity = 5 };

            var isValid = TryValidate(dto, out var errors);

            isValid.Should().BeFalse();
            errors.Should().Contain(e => e.MemberNames.Contains("Name"));
        }

        [Fact]
        public void NegativePrice_ShouldFailValidation()
        {
            var dto = new UpdateProductDto { Name = "Test", Price = -1, StockQuantity = 5 };

            var isValid = TryValidate(dto, out var errors);

            isValid.Should().BeFalse();
            errors.Should().Contain(e => e.MemberNames.Contains("Price"));
        }

        [Fact]
        public void NegativeStockQuantity_ShouldFailValidation()
        {
            var dto = new UpdateProductDto { Name = "Test", Price = 10, StockQuantity = -1 };

            var isValid = TryValidate(dto, out var errors);

            isValid.Should().BeFalse();
            errors.Should().Contain(e => e.MemberNames.Contains("StockQuantity"));
        }

        [Fact]
        public void InvalidCurrencyLength_ShouldFailValidation()
        {
            var dto = new UpdateProductDto { Name = "Test", Price = 10, StockQuantity = 5, Currency = "USDX" };

            var isValid = TryValidate(dto, out var errors);

            isValid.Should().BeFalse();
            errors.Should().Contain(e => e.MemberNames.Contains("Currency"));
        }

        [Fact]
        public void NameExceedsMaxLength_ShouldFailValidation()
        {
            var dto = new UpdateProductDto { Name = new('X', 201), Price = 10, StockQuantity = 5 };

            var isValid = TryValidate(dto, out var errors);

            isValid.Should().BeFalse();
            errors.Should().Contain(e => e.MemberNames.Contains("Name"));
        }
    }

    public class PagedRequestAnnotations
    {
        [Fact]
        public void DefaultValues_ShouldPassValidation()
        {
            var request = new PagedRequest();

            var isValid = TryValidate(request, out var errors);

            isValid.Should().BeTrue();
            errors.Should().BeEmpty();
        }

        [Fact]
        public void PageZero_ShouldFailValidation()
        {
            var request = new PagedRequest { Page = 0 };

            var isValid = TryValidate(request, out var errors);

            isValid.Should().BeFalse();
            errors.Should().Contain(e => e.MemberNames.Contains("Page"));
        }

        [Fact]
        public void PageSizeExceedsMax_ShouldFailValidation()
        {
            var request = new PagedRequest { PageSize = 101 };

            var isValid = TryValidate(request, out var errors);

            isValid.Should().BeFalse();
            errors.Should().Contain(e => e.MemberNames.Contains("PageSize"));
        }

        [Fact]
        public void PageSizeZero_ShouldFailValidation()
        {
            var request = new PagedRequest { PageSize = 0 };

            var isValid = TryValidate(request, out var errors);

            isValid.Should().BeFalse();
            errors.Should().Contain(e => e.MemberNames.Contains("PageSize"));
        }
    }

    public class StockRequestAnnotations
    {
        [Fact]
        public void ValidQuantity_ShouldPassValidation()
        {
            var request = new StockRequest { Quantity = 10 };

            var isValid = TryValidate(request, out var errors);

            isValid.Should().BeTrue();
            errors.Should().BeEmpty();
        }

        [Fact]
        public void ZeroQuantity_ShouldFailValidation()
        {
            var request = new StockRequest { Quantity = 0 };

            var isValid = TryValidate(request, out var errors);

            isValid.Should().BeFalse();
            errors.Should().Contain(e => e.MemberNames.Contains("Quantity"));
        }

        [Fact]
        public void NegativeQuantity_ShouldFailValidation()
        {
            var request = new StockRequest { Quantity = -1 };

            var isValid = TryValidate(request, out var errors);

            isValid.Should().BeFalse();
            errors.Should().Contain(e => e.MemberNames.Contains("Quantity"));
        }
    }
}
