using System.Diagnostics;
using Domain.Data;
using Domain.Entities;

namespace Seeder;

public static class SeatSeeder
{
    private const int Count = 10_000;

    public static async Task<List<Seat>> SeedAsync(TicketingDbContext db, IReadOnlyList<Event> events)
    {
        var sw = Stopwatch.StartNew();
        var sections = new[] { "A", "B", "C", "D", "VIP" };
        var seats = new List<Seat>(Count);
        var seatsPerEvent = Count / events.Count;

        foreach (var ticketEvent in events)
        {
            for (var i = 0; i < seatsPerEvent; i++)
            {
                var section = sections[i % sections.Length];

                seats.Add(new Seat
                {
                    EventId = ticketEvent.Id,
                    Section = section,
                    Row = ((i / 20) % 10 + 1).ToString(),
                    Number = i % 20 + 1,
                    Price = section == "VIP" ? 95m : 25m + i % 8 * 7.5m,
                    Status = "available",
                });
            }
        }

        db.Seats.AddRange(seats);
        await db.SaveChangesAsync();

        sw.Stop();
        Console.WriteLine($"Seeded {seats.Count} seats in {sw.ElapsedMilliseconds} ms.");
        return seats;
    }
}
