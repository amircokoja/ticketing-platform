using System.Diagnostics;
using Domain.Data;
using Domain.Entities;

namespace Seeder;

public static class OrderSeeder
{
    public static async Task SeedAsync(
        TicketingDbContext db,
        IReadOnlyList<Customer> customers,
        IReadOnlyList<Seat> seats
    )
    {
        var sw = Stopwatch.StartNew();
        var random = new Random(3);
        var orderStatuses = new[]
        {
            "paid",
            "paid",
            "paid",
            "paid",
            "pending",
            "cancelled",
            "refunded",
        };

        // 1. Decide which seats are sold, per event.
        //    Each event gets its own sold-rate to create skew.
        var seatsByEvent = seats.GroupBy(s => s.EventId).ToDictionary(g => g.Key, g => g.ToList());
        var soldSeats = new List<Seat>();

        foreach (var (eventId, eventSeats) in seatsByEvent)
        {
            // Sold rate varies per event: 20% to 95%. Hot events sell out, cold ones don't.
            var soldRate = 0.20 + random.NextDouble() * 0.75;
            var toSellCount = (int)(eventSeats.Count * soldRate);

            // Shuffle and take the first N. Each seat is sold at most once.
            Shuffle(eventSeats, random);
            soldSeats.AddRange(eventSeats.Take(toSellCount));

            // Optionally mark a few more as 'held' to give that status some data.
            var heldCount = (int)(eventSeats.Count * 0.05);
            foreach (var s in eventSeats.Skip(toSellCount).Take(heldCount))
                s.Status = "held";
        }

        // 2. Mark the sold seats. We'll attach order_id later via tickets.
        foreach (var s in soldSeats)
            s.Status = "sold";

        // 3. Group sold seats into orders.
        //    Each order = 1-4 tickets, all for the same event (realistic buying behavior).
        var orders = new List<Order>();
        var tickets = new List<Ticket>();
        var remainingByEvent = soldSeats
            .GroupBy(s => s.EventId)
            .ToDictionary(g => g.Key, g => new Queue<Seat>(g));

        while (remainingByEvent.Values.Any(q => q.Count > 0))
        {
            // Pick an event that still has unassigned sold seats.
            var eventId = remainingByEvent.First(kv => kv.Value.Count > 0).Key;
            var queue = remainingByEvent[eventId];

            var ticketsInThisOrder = Math.Min(random.Next(1, 5), queue.Count);
            var orderSeats = new List<Seat>();
            for (var i = 0; i < ticketsInThisOrder; i++)
                orderSeats.Add(queue.Dequeue());

            var order = new Order
            {
                CustomerId = customers[random.Next(customers.Count)].Id,
                Status = orderStatuses[random.Next(orderStatuses.Length)],
                CreatedAt = DateTime
                    .UtcNow.AddDays(-random.Next(0, 365))
                    .AddMinutes(-random.Next(0, 1440)),
                Total = orderSeats.Sum(s => s.Price),
            };

            orders.Add(order);

            foreach (var seat in orderSeats)
            {
                tickets.Add(
                    new Ticket
                    {
                        EventId = eventId,
                        SeatId = seat.Id,
                        Order = order, // EF will resolve order.Id after SaveChanges
                        PurchasedAt = order.CreatedAt.AddMinutes(random.Next(1, 120)),
                    }
                );
            }
        }

        db.Orders.AddRange(orders);
        db.Tickets.AddRange(tickets);
        await db.SaveChangesAsync();

        sw.Stop();
        Console.WriteLine(
            $"Seeded {orders.Count} orders and {tickets.Count} tickets in {sw.ElapsedMilliseconds} ms."
        );
    }

    private static void Shuffle<T>(IList<T> items, Random random)
    {
        for (var i = items.Count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (items[i], items[j]) = (items[j], items[i]);
        }
    }
}
