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
    public class PaymentAttemptConfiguration : IEntityTypeConfiguration<PaymentAttempt>
    {
        public void Configure(EntityTypeBuilder<PaymentAttempt> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.PaymentId).IsRequired();
            builder.Property(p => p.UserId).IsRequired();
            builder.Property(p => p.AttemptNumber).IsRequired();
            builder.Property(p => p.CurrentAttemptStatus).IsRequired();
            builder.Property(p => p.ErrorMessage);
            builder.Property(p => p.ProviderTransactionId).IsRequired();
            builder.Property(p => p.StartedAt).IsRequired();
            builder.Property(p => p.CompletedAt);
     
        }
    }
}
