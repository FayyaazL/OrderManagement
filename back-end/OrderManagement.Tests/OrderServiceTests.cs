using FluentAssertions;
using NSubstitute;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Tests;

public class OrderServiceTests
{
    private readonly IOrderRepository _repository;
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _repository = Substitute.For<IOrderRepository>();
        _service = new OrderService(_repository);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateAndPersistOrder()
    {
        var request = new CreateOrderRequest(
            "Contoso",
            "ORD-1001",
            [
                new OrderLineRequest("Keyboard", 2, 49.99m)
            ]);

        _repository
            .GetByExternalReferenceAsync(
                request.ExternalReference,
                Arg.Any<CancellationToken>())
            .Returns((Order?)null);

        var result = await _service.CreateAsync(request);

        result.CustomerName.Should().Be("Contoso");
        result.ExternalReference.Should().Be("ORD-1001");
        result.Status.Should().Be(OrderStatus.Pending);
        result.Lines.Should().ContainSingle();
        result.Total.Should().Be(99.98m);

        await _repository.Received(1).AddAsync(
            Arg.Is<Order>(order =>
                order.CustomerName == "Contoso" &&
                order.ExternalReference == "ORD-1001" &&
                order.Status == OrderStatus.Pending &&
                order.Total == 99.98m),
            Arg.Any<CancellationToken>());

        await _repository.Received(1).SaveChangesAsync(
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ShouldRejectDuplicateExternalReference()
    {
        var existingOrder = new Order(
            "Existing customer",
            "ORD-1001",
            [new OrderLine("Keyboard", 1, 10m)]);

        var request = new CreateOrderRequest(
            "Contoso",
            "ORD-1001",
            [
                new OrderLineRequest("Mouse", 1, 20m)
            ]);

        _repository
            .GetByExternalReferenceAsync(
                request.ExternalReference,
                Arg.Any<CancellationToken>())
            .Returns(existingOrder);

        var action = () => _service.CreateAsync(request);

        await action.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("*ORD-1001*");

        await _repository.DidNotReceive().AddAsync(
            Arg.Any<Order>(),
            Arg.Any<CancellationToken>());

        await _repository.DidNotReceive().SaveChangesAsync(
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMappedOrder()
    {
        var order = new Order(
            "Contoso",
            "ORD-1001",
            [new OrderLine("Keyboard", 2, 49.99m)]);

        _repository
            .GetByIdAsync(order.Id, Arg.Any<CancellationToken>())
            .Returns(order);

        var result = await _service.GetByIdAsync(order.Id);

        result.Id.Should().Be(order.Id);
        result.CustomerName.Should().Be("Contoso");
        result.Total.Should().Be(99.98m);
        result.Lines.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowWhenOrderDoesNotExist()
    {
        var id = Guid.NewGuid();

        _repository
            .GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns((Order?)null);

        var action = () => _service.GetByIdAsync(id);

        await action.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Order '{id}' was not found.");
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldApplyValidStatusTransition()
    {
        var order = new Order(
            "Contoso",
            "ORD-1001",
            [new OrderLine("Keyboard", 1, 10m)]);

        _repository
            .GetByIdAsync(order.Id, Arg.Any<CancellationToken>())
            .Returns(order);

        var result = await _service.UpdateStatusAsync(
            order.Id,
            new UpdateOrderStatusRequest(OrderStatus.Confirmed));

        result.Status.Should().Be(OrderStatus.Confirmed);
        order.Status.Should().Be(OrderStatus.Confirmed);

        await _repository.Received(1).SaveChangesAsync(
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldRejectInvalidStatusTransition()
    {
        var order = new Order(
            "Contoso",
            "ORD-1001",
            [new OrderLine("Keyboard", 1, 10m)]);

        _repository
            .GetByIdAsync(order.Id, Arg.Any<CancellationToken>())
            .Returns(order);

        var action = () => _service.UpdateStatusAsync(
            order.Id,
            new UpdateOrderStatusRequest(OrderStatus.Fulfilled));

        await action.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("*Pending*Fulfilled*");

        await _repository.DidNotReceive().SaveChangesAsync(
            Arg.Any<CancellationToken>());
    }
}