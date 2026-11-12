using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken = default);

    Task<Order?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Order?> GetByExternalReferenceAsync(
        string externalReference,
        CancellationToken cancellationToken = default);

    Task<List<Order>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}