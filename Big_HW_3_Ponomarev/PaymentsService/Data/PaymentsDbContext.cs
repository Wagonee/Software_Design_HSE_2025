using Microsoft.EntityFrameworkCore;
using PaymentsService.Data.Entities;
using PaymentsService.Inbox;
using PaymentsService.Outbox;

namespace PaymentsService.Data;

public class PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
            .HasIndex(a => a.UserId)
            .IsUnique();
        
        modelBuilder.Entity<OutboxMessage>().HasKey(x => x.Id);
    }
}