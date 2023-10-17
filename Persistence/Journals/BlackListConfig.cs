using Entities.Journals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Journals;

public class BlackListConfig : IEntityTypeConfiguration<BlackList>
{
    public void Configure(EntityTypeBuilder<BlackList> builder)
    {
        builder.HasIndex(i => i.Id);
        builder.HasIndex(i => i.JournalId);
        builder.HasIndex(i => i.FromDate);
    }
}