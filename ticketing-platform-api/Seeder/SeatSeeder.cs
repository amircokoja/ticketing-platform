using System.Diagnostics;
using Domain.Data;
using Domain.Entities;

namespace Seeder;

public static class SeatSeeder
{
    public static async Task<List<Seat>> SeedAsync(
        TicketingDbContext db,
        IReadOnlyList<Event> events
    )
    {
        var sw = Stopwatch.StartNew();
        var random = new Random(4);
        var sections = new[] { "VIP", "A", "B", "C", "D" };

        var seats = new List<Seat>();

        foreach (var ev in events)
        {
            // Each event has between 1,500 and 5,000 seats — captures the
            // small-club-to-stadium variety. Skew matters for query plans.
            var seatCount = random.Next(1500, 5001);

            for (var i = 0; i < seatCount; i++)
            {
                var section = sections[i * sections.Length / seatCount]; // VIP first, then A..D
                var basePrice = section switch
                {
                    "VIP" => 200m,
                    "A" => 120m,
                    "B" => 80m,
                    "C" => 50m,
                    _ => 30m,
                };

                seats.Add(
                    new Seat
                    {
                        EventId = ev.Id,
                        Section = section,
                        Row = ((i / 20) % 50 + 1).ToString(),
                        Number = i % 20 + 1,
                        Price = basePrice + random.Next(0, 20),
                        Status = "available", // start all available; OrderSeeder flips some to sold
                    }
                );
            }
        }

        db.Seats.AddRange(seats);
        await db.SaveChangesAsync();

        sw.Stop();
        Console.WriteLine($"Seeded {seats.Count} seats in {sw.ElapsedMilliseconds} ms.");
        return seats;
    }
}
