using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Services;

public class OrderService
{
    private readonly IOrderRepository _repository;

    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByExternalReferenceAsync(
            request.ExternalReference,
            cancellationToken);

        if (existing is not null)
        {
            throw new InvalidOperationException(
                $"An order with external reference '{request.ExternalReference}' already exists.");
        }

        var lines = request.Lines.Select(line =>
            new OrderLine(line.ProductName, line.Quantity, line.UnitPrice));

        var order = new Order(
            request.CustomerName,
            request.ExternalReference,
            lines);

        await _repository.AddAsync(order, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToResponse(order);
    }

    public async Task<List<OrderResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var orders = await _repository.GetAllAsync(cancellationToken);

        return orders.Select(MapToResponse).ToList();
    }

    public async Task<OrderResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetByIdAsync(id, cancellationToken);

        if (order is null)
            throw new KeyNotFoundException($"Order '{id}' was not found.");

        return MapToResponse(order);
    }

    public async Task<OrderResponse> UpdateStatusAsync(
        Guid id,
        UpdateOrderStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetByIdAsync(id, cancellationToken);

        if (order is null)
            throw new KeyNotFoundException($"Order '{id}' was not found.");

        order.ChangeStatus(request.Status);

        await _repository.SaveChangesAsync(cancellationToken);

        return MapToResponse(order);
    }

    private static OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse(
            order.Id,
            order.CustomerName,
            order.ExternalReference,
            order.CreatedAtUtc,
            order.Status,
            order.Lines.Select(line => new OrderLineResponse(
                line.Id,
                line.ProductName,
                line.Quantity,
                line.UnitPrice,
                line.LineTotal)).ToList(),
            order.Total);
    }
}