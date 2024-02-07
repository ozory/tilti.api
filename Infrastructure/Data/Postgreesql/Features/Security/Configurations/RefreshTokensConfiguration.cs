using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data.Postgreesql.Features.Security.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Postgreesql.Features.Security.Configurations
{
    public class RefreshTokensConfiguration : IEntityTypeConfiguration<RefreshTokens>
    {

        public void Configure(EntityTypeBuilder<RefreshTokens> builder)
        {
            builder.ToTable("RefeshTokens", "security");

            builder.HasKey(b => b.Id);

            builder.Property(x => x.LastLogin)
                .HasColumnName("LastLogin")
                .HasColumnType("timestamp");

            builder.Property(x => x.RefreshToken)
                .HasColumnName("RefeshToken")
                .HasMaxLength(250)
                .IsRequired(true);

            builder.HasOne(e => e.User)
                .WithOne();
        }
    }
}