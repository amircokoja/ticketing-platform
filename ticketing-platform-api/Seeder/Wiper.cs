using Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Seeder;

public static class Wiper
{
    public static async Task WipeAsync(TicketingDbContext db)
    {
        Console.Write("Wiping existing data... ");
        await db.Database.ExecuteSqlRawAsync(
            """
                TRUNCATE TABLE tickets, orders, seats, events, customers, venues
                RESTART IDENTITY CASCADE;
            """
        );
        Console.WriteLine("done.");
    }
}
