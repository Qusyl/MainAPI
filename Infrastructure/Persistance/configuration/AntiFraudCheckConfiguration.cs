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
    public class AntiFraudCheckConfiguration : IEntityTypeConfiguration<FraudCheck>
    {
        public void Configure(EntityTypeBuilder<FraudCheck> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.RuleDecision).IsRequired();
            builder.Property(x => x.RuleName).IsRequired();
            builder.Property(x => x.Reason);
            builder.Property(x => x.CreatedAt).IsRequired();

        }
    }
}
