using Entities.Journals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Journals;

public class JournalConfig : IEntityTypeConfiguration<Journal>
{
    public void Configure(EntityTypeBuilder<Journal> builder)
    {
        builder.HasIndex(i => i.Id);
        builder.HasIndex(i => i.Title);
        builder.HasIndex(i => i.Issn);
        builder.HasIndex(i => i.EIssn);
    }
}