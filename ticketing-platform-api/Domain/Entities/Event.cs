namespace Domain.Entities;

public class Event
{
    public long Id { get; set; }
    public long VenueId { get; set; }
    public Venue Venue { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public DateTime StartsAt { get; set; }
    public string Status { get; set; } = "scheduled";

    public List<Seat> Seats { get; set; } = [];
}
