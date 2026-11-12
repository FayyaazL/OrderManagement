using FluentAssertions;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Tests;

public class OrderLineTests
{
    [Fact]
    public void Constructor_ShouldCalculateLineTotal()
    {
        var line = new OrderLine("Keyboard", 2, 49.99m);

        line.LineTotal.Should().Be(99.98m);
    }

    [Theory]
    [InlineData("", 1, 10)]
    [InlineData("Keyboard", 0, 10)]
    [InlineData("Keyboard", -1, 10)]
    [InlineData("Keyboard", 1, -1)]
    public void Constructor_ShouldRejectInvalidValues(
        string productName,
        int quantity,
        decimal unitPrice)
    {
        var action = () => new OrderLine(
            productName,
            quantity,
            unitPrice);

        action.Should().Throw<ArgumentException>();
    }
}