using Entities.Journals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Journals;

public class JournalRecordConfig : IEntityTypeConfiguration<JournalRecord>
{
    public void Configure(EntityTypeBuilder<JournalRecord> builder)
    {
        builder.HasIndex(x => x.Index);
        builder.HasIndex(x => x.Type);
        builder.HasIndex(x => x.Value);
        builder.HasIndex(x => x.IscClass);
        builder.HasIndex(x => x.QRank);
    }
}