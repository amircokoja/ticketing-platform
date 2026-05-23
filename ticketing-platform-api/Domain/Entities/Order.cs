namespace Domain.Entities;

public class Order
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public decimal Total { get; set; }
    public string Status { get; set; } = "pending"; // pending, paid, cancelled, refunded
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Ticket> Tickets { get; set; } = [];
}
