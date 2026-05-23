namespace Domain.Entities;

public class Seat
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public Event Event { get; set; } = null!;

    public string Section { get; set; } = string.Empty; // "A", "B", "VIP", etc.
    public string Row { get; set; } = string.Empty; // "1", "2", "AA", etc.
    public int Number { get; set; } // seat number within row
    public decimal Price { get; set; }
    public string Status { get; set; } = "available"; // available, held, sold
}
