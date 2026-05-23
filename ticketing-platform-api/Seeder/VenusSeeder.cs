using System.Diagnostics;
using Domain.Data;
using Domain.Entities;

namespace Seeder;

public static class VenueSeeder
{
    public static async Task<List<Venue>> SeedAsync(TicketingDbContext db)
    {
        var sw = Stopwatch.StartNew();

        var venues = new List<Venue>
        {
            new()
            {
                Name = "Old Trafford",
                City = "Manchester",
                Capacity = 74000,
            },
            new()
            {
                Name = "Madison Square Garden",
                City = "New York",
                Capacity = 20000,
            },
            new()
            {
                Name = "Royal Albert Hall",
                City = "London",
                Capacity = 5300,
            },
            new()
            {
                Name = "Sarajevo Arena",
                City = "Sarajevo",
                Capacity = 12000,
            },
            new()
            {
                Name = "Berlin Olympiastadion",
                City = "Berlin",
                Capacity = 74000,
            },
            new()
            {
                Name = "The Troubadour",
                City = "Los Angeles",
                Capacity = 500,
            },
            new()
            {
                Name = "Tokyo Dome",
                City = "Tokyo",
                Capacity = 55000,
            },
            new()
            {
                Name = "Vogošća Cultural Hall",
                City = "Vogošća",
                Capacity = 800,
            },
        };

        db.Venues.AddRange(venues);
        await db.SaveChangesAsync();

        sw.Stop();
        Console.WriteLine($"Seeded {venues.Count} venues in {sw.ElapsedMilliseconds} ms.");
        return venues;
    }
}
