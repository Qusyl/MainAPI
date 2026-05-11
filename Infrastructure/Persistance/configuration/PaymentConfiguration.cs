using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.configuration
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.UserId).IsRequired();
            builder.Property(b => b.Currency).IsRequired();
            builder.Property(b => b.Amount).IsRequired();
            builder.Property(b => b.Status).IsRequired();
            builder.Property(b => b.CurrentProvider);
            builder.Property(b => b.Version).IsRowVersion();
            builder.Property(b => b.IdempotencyKey).IsRequired();
            builder.HasIndex(b => b.IdempotencyKey).IsUnique();
            builder.Property(b => b.CreatedAt).IsRequired();
            builder.Property(b => b.UpdatedAt).IsRequired();

        }
    }
}
