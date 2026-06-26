using FluentAssertions;
using testNet.Application.Common;

namespace testNet.Tests.Application;

public class PagedResultTests
{
    [Fact]
    public void TotalPages_ShouldCeilDivisionOfTotalCountByPageSize()
    {
        var result = new PagedResult<object> { Page = 1, PageSize = 10, TotalCount = 25 };

        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public void TotalPages_WhenTotalCountIsZero_ShouldBeZero()
    {
        var result = new PagedResult<object> { Page = 1, PageSize = 10, TotalCount = 0 };

        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public void HasPreviousPage_WhenPageGreaterThanOne_ShouldBeTrue()
    {
        var result = new PagedResult<object> { Page = 2, PageSize = 10, TotalCount = 25 };

        result.HasPreviousPage.Should().BeTrue();
    }

    [Fact]
    public void HasPreviousPage_WhenPageIsOne_ShouldBeFalse()
    {
        var result = new PagedResult<object> { Page = 1, PageSize = 10, TotalCount = 25 };

        result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public void HasNextPage_WhenMorePagesAvailable_ShouldBeTrue()
    {
        var result = new PagedResult<object> { Page = 1, PageSize = 10, TotalCount = 25 };

        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void HasNextPage_WhenOnLastPage_ShouldBeFalse()
    {
        var result = new PagedResult<object> { Page = 3, PageSize = 10, TotalCount = 25 };

        result.HasNextPage.Should().BeFalse();
    }
}