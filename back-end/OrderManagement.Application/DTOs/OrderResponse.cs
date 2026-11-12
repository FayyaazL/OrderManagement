using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.DTOs;

public record OrderResponse(
    Guid Id,
    string CustomerName,
    string ExternalReference,
    DateTime CreatedAtUtc,
    OrderStatus Status,
    List<OrderLineResponse> Lines,
    decimal Total);