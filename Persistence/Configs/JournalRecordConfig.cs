using Entities.Journals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class JournalRecordConfig : IEntityTypeConfiguration<JournalRecord>
{
    public void Configure(EntityTypeBuilder<JournalRecord> builder)
    {
        builder.HasIndex(i => i.Year);
        builder.HasIndex(i => i.Index);
        builder.HasIndex(i => i.QRank);
        
        builder.Property(i => i.NormalizedCategory).HasMaxLength(120);
        builder.HasIndex(i => i.NormalizedCategory);

        builder.Property(i => i.If).SetAsImpactFactor();
        builder.Property(i => i.Mif).SetAsImpactFactor();
        builder.Property(i => i.Aif).SetAsImpactFactor();
    }
}