using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;
using OrderManagement.Infrastructure.Data;

namespace OrderManagement.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Order order,
        CancellationToken cancellationToken = default)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
    }

    public Task<Order?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Orders
            .Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public Task<Order?> GetByExternalReferenceAsync(
        string externalReference,
        CancellationToken cancellationToken = default)
    {
        return _context.Orders
            .Include(o => o.Lines)
            .FirstOrDefaultAsync(
                o => o.ExternalReference == externalReference,
                cancellationToken);
    }

    public Task<List<Order>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.Orders
            .Include(o => o.Lines)
            .OrderByDescending(o => o.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}