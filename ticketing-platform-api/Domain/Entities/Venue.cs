namespace Domain.Entities;

public class Venue
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int Capacity { get; set; }

    public List<Event> Events { get; set; } = [];
}
