using System.Diagnostics;
using Domain.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Seeder;

public static class OrderSeeder
{
    private const int OrderCount = 50_000;
    private const int TicketCount = 100_000;

    public static async Task SeedAsync(
        TicketingDbContext db,
        IReadOnlyList<Customer> customers,
        IReadOnlyList<Event> events,
        IReadOnlyList<Seat> seats
    )
    {
        var sw = Stopwatch.StartNew();
        var random = new Random(3);
        var statuses = new[] { "paid", "paid", "paid", "paid", "pending", "cancelled", "refunded" };

        var orders = Enumerable
            .Range(0, OrderCount)
            .Select(_ => new Order
            {
                CustomerId = customers[random.Next(customers.Count)].Id,
                Status = statuses[random.Next(statuses.Length)],
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 365)).AddMinutes(-random.Next(0, 1440)),
                Total = 0m,
            })
            .ToList();

        db.Orders.AddRange(orders);
        await db.SaveChangesAsync();

        var seatsByEvent = seats
            .GroupBy(seat => seat.EventId)
            .ToDictionary(group => group.Key, group => group.ToList());

        var shuffledOrders = orders.ToList();
        Shuffle(shuffledOrders, random);

        var tickets = new List<Ticket>(TicketCount);
        for (var i = 0; i < TicketCount; i++)
        {
            var ticketEvent = events[random.Next(events.Count)];
            var eventSeats = seatsByEvent[ticketEvent.Id];
            var seat = eventSeats[random.Next(eventSeats.Count)];
            var order = shuffledOrders[i % shuffledOrders.Count];

            tickets.Add(new Ticket
            {
                EventId = ticketEvent.Id,
                SeatId = seat.Id,
                OrderId = order.Id,
                PurchasedAt = order.CreatedAt.AddMinutes(random.Next(1, 120)),
            });
        }

        db.Tickets.AddRange(tickets);
        await db.SaveChangesAsync();

        await db.Database.ExecuteSqlRawAsync(
            """
            UPDATE seats
            SET status = CASE
                WHEN random() < 0.83 THEN 'sold'
                WHEN random() < 0.92 THEN 'held'
                ELSE 'available'
            END;

            UPDATE orders o
            SET total = totals.total
            FROM (
                SELECT t.order_id, SUM(s.price) AS total
                FROM tickets t
                JOIN seats s ON s.id = t.seat_id
                GROUP BY t.order_id
            ) totals
            WHERE totals.order_id = o.id;
            """
        );

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
