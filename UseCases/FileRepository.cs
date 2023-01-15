using Frameworks;
using Persistence;
using UseCases;
using File = Entities.File;

namespace Services.Repositories;

public class FileRepository : BaseRepository<FileContext, File>, IFileRepository
{
    public FileRepository(FileContext fileContext)
        : base(fileContext)
    {
    }
}