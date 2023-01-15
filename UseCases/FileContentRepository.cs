using Entities;
using Frameworks;
using Persistence;
using UseCases;

namespace Services.Repositories;

public class FileContentRepository : BaseRepository<FileContext, FileContent>, IFileContentRepository
{
    public FileContentRepository(FileContext fileContext)
        : base(fileContext)
    {
    }
}