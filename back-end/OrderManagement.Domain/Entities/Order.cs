using OrderManagement.Domain.Enums;

namespace OrderManagement.Domain.Entities;

public class Order
{
    private readonly List<OrderLine> _lines = new();

    public Guid Id { get; private set; }

    public string CustomerName { get; private set; } = string.Empty;

    public string ExternalReference { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }

    public OrderStatus Status { get; private set; }

    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();

    public decimal Total => _lines.Sum(line => line.LineTotal);

    private Order() { } // For EF Core

    public Order(
        string customerName,
        string externalReference,
        IEnumerable<OrderLine> lines)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Customer name is required.");

        if (string.IsNullOrWhiteSpace(externalReference))
            throw new ArgumentException("External reference is required.");

        var orderLines = lines?.ToList()
            ?? throw new ArgumentException("Order lines are required.");

        if (orderLines.Count == 0)
            throw new ArgumentException("An order must contain at least one line.");

        Id = Guid.NewGuid();
        CustomerName = customerName.Trim();
        ExternalReference = externalReference.Trim();
        CreatedAtUtc = DateTime.UtcNow;
        Status = OrderStatus.Pending;

        _lines.AddRange(orderLines);
    }

    public void ChangeStatus(OrderStatus newStatus)
    {
        var validTransition = (Status, newStatus) switch
        {
            (OrderStatus.Pending, OrderStatus.Confirmed) => true,
            (OrderStatus.Pending, OrderStatus.Cancelled) => true,
            (OrderStatus.Confirmed, OrderStatus.Fulfilled) => true,
            (OrderStatus.Confirmed, OrderStatus.Cancelled) => true,
            _ => false
        };

        if (!validTransition)
        {
            throw new InvalidOperationException(
                $"Cannot change order status from {Status} to {newStatus}.");
        }

        Status = newStatus;
    }
}