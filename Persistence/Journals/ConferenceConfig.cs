using Entities.Conferences;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Journals;

public class ConferenceConfig : IEntityTypeConfiguration<Conference>
{
    public void Configure(EntityTypeBuilder<Conference> builder)
    {
        builder.HasIndex(i => i.Id);
        builder.HasIndex(i => i.Title);
        builder.HasIndex(i => i.TitleEn);
    }
}