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
    public class ErrorAuditConfiguration : IEntityTypeConfiguration<ErrorAudit>
    {
        public void Configure(EntityTypeBuilder<ErrorAudit> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.PaymentId).IsRequired();
            builder.Property(b => b.OccuredOn).IsRequired();

        }
    }
}
