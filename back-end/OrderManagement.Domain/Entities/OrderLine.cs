namespace OrderManagement.Domain.Entities;

public class OrderLine
{
    public Guid Id { get; private set; }

    public Guid OrderId { get; private set; }

    public string ProductName { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal LineTotal => Quantity * UnitPrice;

    private OrderLine() { } // For EF Core

    public OrderLine(string productName, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name is required.");

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative.");

        Id = Guid.NewGuid();
        ProductName = productName.Trim();
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}