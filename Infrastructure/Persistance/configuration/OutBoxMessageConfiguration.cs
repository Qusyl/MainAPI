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
    public class OutBoxMessageConfiguration : IEntityTypeConfiguration<OutBoxMessage>
    {
        public void Configure(EntityTypeBuilder<OutBoxMessage> builder)
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Type).IsRequired();
            builder.Property(m => m.Payload).IsRequired();
            builder.Property(m => m.Version).IsRequired();
            builder.Property(m => m.OccurredOn).IsRequired();
            builder.Property(m => m.ProcessedOn);
            builder.Property(m => m.Error);
        }
    }
}
