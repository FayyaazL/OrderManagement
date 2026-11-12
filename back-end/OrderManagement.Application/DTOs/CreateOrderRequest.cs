namespace OrderManagement.Application.DTOs;

public record CreateOrderRequest(
    string CustomerName,
    string ExternalReference,
    List<OrderLineRequest> Lines);