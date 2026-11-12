using FluentAssertions;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Tests;

public class OrderTests
{
    [Fact]
    public void Constructor_ShouldCreatePendingOrderWithLines()
    {
        var line = new OrderLine("Keyboard", 2, 49.99m);

        var order = new Order(
            "TestCustomer",
            "ORD-1001",
            [line]);

        order.CustomerName.Should().Be("TestCustomer");
        order.ExternalReference.Should().Be("ORD-1001");
        order.Status.Should().Be(OrderStatus.Pending);
        order.Lines.Should().ContainSingle().Which.Should().Be(line);
        order.Total.Should().Be(99.98m);
        order.CreatedAtUtc.Should().BeCloseTo(
            DateTime.UtcNow,
            TimeSpan.FromSeconds(2));
    }

    [Theory]
    [InlineData("", "ORD-1001")]
    [InlineData("   ", "ORD-1001")]
    [InlineData("TestCustomer", "")]
    [InlineData("TestCustomer", "   ")]
    public void Constructor_ShouldRejectMissingRequiredValues(
        string customerName,
        string externalReference)
    {
        var action = () => new Order(
            customerName,
            externalReference,
            [new OrderLine("Keyboard", 1, 10m)]);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_ShouldRejectOrderWithoutLines()
    {
        var action = () => new Order(
            "TestCustomer",
            "ORD-1001",
            []);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("An order must contain at least one line.");
    }

    [Fact]
    public void ChangeStatus_ShouldAllowPendingToConfirmed()
    {
        var order = CreateOrder();

        order.ChangeStatus(OrderStatus.Confirmed);

        order.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public void ChangeStatus_ShouldAllowPendingToCancelled()
    {
        var order = CreateOrder();

        order.ChangeStatus(OrderStatus.Cancelled);

        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void ChangeStatus_ShouldAllowConfirmedToFulfilled()
    {
        var order = CreateOrder();
        order.ChangeStatus(OrderStatus.Confirmed);

        order.ChangeStatus(OrderStatus.Fulfilled);

        order.Status.Should().Be(OrderStatus.Fulfilled);
    }

    [Fact]
    public void ChangeStatus_ShouldRejectPendingToFulfilled()
    {
        var order = CreateOrder();

        var action = () => order.ChangeStatus(OrderStatus.Fulfilled);

        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Cannot change order status from Pending to Fulfilled.");
    }

    [Theory]
    [InlineData(OrderStatus.Cancelled, OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Cancelled, OrderStatus.Fulfilled)]
    [InlineData(OrderStatus.Fulfilled, OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Fulfilled, OrderStatus.Pending)]
    public void ChangeStatus_ShouldRejectInvalidTransitions(
        OrderStatus currentStatus,
        OrderStatus newStatus)
    {
        var order = CreateOrder();

        if (currentStatus == OrderStatus.Cancelled)
        {
            order.ChangeStatus(OrderStatus.Cancelled);
        }
        else if (currentStatus == OrderStatus.Fulfilled)
        {
            order.ChangeStatus(OrderStatus.Confirmed);
            order.ChangeStatus(OrderStatus.Fulfilled);
        }

        var action = () => order.ChangeStatus(newStatus);

        action.Should().Throw<InvalidOperationException>();
    }

    private static Order CreateOrder()
    {
        return new Order(
            customerName: "TestCustomer",
            externalReference: "ORD-1001",
            lines: [new OrderLine("Keyboard", 2, 49.99m)]);
    }
}