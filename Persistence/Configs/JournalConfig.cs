using Entities.Journals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class JournalConfig : IEntityTypeConfiguration<Journal>
{
    public void Configure(EntityTypeBuilder<Journal> builder)
    {
        builder.Property(i => i.NormalizedTitle).HasMaxLength(120);
        builder.HasIndex(i => i.NormalizedTitle);
        
        builder.Property(i => i.Issn).HasMaxLength(8);
        builder.HasIndex(i => i.Issn).IsUnique().HasFilter("[Issn] IS NOT NULL");
        
        builder.Property(i => i.EIssn).HasMaxLength(8);
        builder.HasIndex(i => i.EIssn).IsUnique();
    }
}