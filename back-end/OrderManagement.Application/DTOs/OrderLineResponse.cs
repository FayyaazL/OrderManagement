namespace OrderManagement.Application.DTOs;

public record OrderLineResponse(
    Guid Id,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal);