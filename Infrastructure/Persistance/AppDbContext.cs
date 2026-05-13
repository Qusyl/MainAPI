using Domain.Entity;
using Infrastructure.Persistance.configuration;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Persistance
{
    public class AppDbContext : DbContext
    {
        public DbSet<Payment> Payments => Set<Payment>();

        public DbSet<OutBoxMessage> OutBoxMessages => Set<OutBoxMessage>();

        public DbSet<PaymentAttempt> Attempts => Set<PaymentAttempt>();

        public DbSet<ErrorAudit> Audits => Set<ErrorAudit>();

        public DbSet<FraudCheck> Checks => Set<FraudCheck>();


        public DbSet<User> Users => Set<User>();
        public AppDbContext()
        {
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new OutBoxMessageConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentAttemptConfiguration());
            modelBuilder.ApplyConfiguration(new ErrorAuditConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new AntiFraudCheckConfiguration());
        }
    }
}
