using System.Diagnostics;
using Domain.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Seeder;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.local.json", optional: true, reloadOnChange: true)
    .Build();

var connectionString =
    config.GetConnectionString("Ticketing")
    ?? throw new InvalidOperationException("Connection string 'Ticketing' missing.");

var options = new DbContextOptionsBuilder<TicketingDbContext>()
    .UseNpgsql(connectionString)
    .UseSnakeCaseNamingConvention()
    .Options;

await using var db = new TicketingDbContext(options);

Console.WriteLine("Seeding ticketing database...");
var total = Stopwatch.StartNew();

await Wiper.WipeAsync(db);
var venues = await VenueSeeder.SeedAsync(db);
var customers = await CustomerSeeder.SeedAsync(db);
var events = await EventSeeder.SeedAsync(db, venues);
var seats = await SeatSeeder.SeedAsync(db, events);
await OrderSeeder.SeedAsync(db, customers, seats);

total.Stop();
Console.WriteLine($"\nDone in {total.Elapsed.TotalSeconds:F1}s.");
