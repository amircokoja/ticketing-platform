namespace Domain.Entities;

public class Ticket
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public Event Event { get; set; } = null!;

    public long SeatId { get; set; }
    public Seat Seat { get; set; } = null!;

    public long OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
}
