using Domain.Entity;
using Infrastructure.Persistance.configuration;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Persistance
{
    public class AppDbContext : DbContext
    {
        public DbSet<Payment> Payments => Set<Payment>();

        public DbSet<OutBoxMessage> OutBoxMessages => Set<OutBoxMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
        }
    }
}
