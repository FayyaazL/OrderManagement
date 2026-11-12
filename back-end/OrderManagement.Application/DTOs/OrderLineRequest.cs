namespace OrderManagement.Application.DTOs;

public record OrderLineRequest(
    string ProductName,
    int Quantity,
    decimal UnitPrice);