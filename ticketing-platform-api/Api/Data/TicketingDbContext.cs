using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class TicketingDbContext(DbContextOptions<TicketingDbContext> options)
    : DbContext(options) { }
