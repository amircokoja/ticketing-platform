using System.Diagnostics;
using Bogus;
using Domain.Data;
using Domain.Entities;

namespace Seeder;

public static class CustomerSeeder
{
    private const int Count = 3_000;

    public static async Task<List<Customer>> SeedAsync(TicketingDbContext db)
    {
        var sw = Stopwatch.StartNew();

        var faker = new Faker<Customer>()
            .UseSeed(1)
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, (f, c) => f.Internet.Email(c.Name).ToLowerInvariant())
            .RuleFor(c => c.CreatedAt, f => f.Date.PastOffset(2).UtcDateTime);

        var customers = faker.Generate(Count);

        // Bogus can produce duplicate emails. Customer.Email is unique — dedupe.
        customers = customers.GroupBy(c => c.Email).Select(g => g.First()).ToList();

        db.Customers.AddRange(customers);
        await db.SaveChangesAsync();

        sw.Stop();
        Console.WriteLine($"Seeded {customers.Count} customers in {sw.ElapsedMilliseconds} ms.");
        return customers;
    }
}
