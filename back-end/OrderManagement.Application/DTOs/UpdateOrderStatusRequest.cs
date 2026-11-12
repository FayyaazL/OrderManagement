using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.DTOs;

public record UpdateOrderStatusRequest(OrderStatus Status);