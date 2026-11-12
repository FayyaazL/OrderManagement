using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Services;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.CreateAsync(
            request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = order.Id },
            order);
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetAllAsync(cancellationToken);
        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.GetByIdAsync(id, cancellationToken);
        return Ok(order);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<OrderResponse>> UpdateStatus(
        Guid id,
        UpdateOrderStatusRequest request,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.UpdateStatusAsync(
            id, request, cancellationToken);

        return Ok(order);
    }
}