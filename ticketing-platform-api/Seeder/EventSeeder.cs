using System.Diagnostics;
using Bogus;
using Domain.Data;
using Domain.Entities;

namespace Seeder;

public static class EventSeeder
{
    private const int Count = 50;

    private static readonly string[] Adjectives =
    [
        "Summer",
        "Winter",
        "Acoustic",
        "Electric",
        "Underground",
        "Grand",
        "Late Night",
        "Annual",
    ];

    private static readonly string[] Themes =
    [
        "Jazz Festival",
        "Symphony",
        "Tech Conference",
        "Comedy Night",
        "Rock Concert",
        "Folk Gathering",
        "DJ Set",
        "Theatre Premiere",
        "Championship Match",
        "Album Release",
    ];

    public static async Task<List<Event>> SeedAsync(TicketingDbContext db, List<Venue> venues)
    {
        var sw = Stopwatch.StartNew();

        var faker = new Faker<Event>()
            .UseSeed(2)
            .RuleFor(
                e => e.Name,
                f =>
                    $"{f.PickRandom(Adjectives)} {f.PickRandom(Themes)} {f.Random.Number(2024, 2027)}"
            )
            .RuleFor(e => e.VenueId, f => f.PickRandom(venues).Id)
            .RuleFor(e => e.StartsAt, f => f.Date.FutureOffset(1).UtcDateTime)
            .RuleFor(
                e => e.Status,
                f => f.PickRandom("on_sale", "on_sale", "on_sale", "sold_out", "scheduled")
            );

        var events = faker.Generate(Count);

        db.Events.AddRange(events);
        await db.SaveChangesAsync();

        sw.Stop();
        Console.WriteLine($"Seeded {events.Count} events in {sw.ElapsedMilliseconds} ms.");
        return events;
    }
}
