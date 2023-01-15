using System.Security.Cryptography;
using Entities;
using Framework;
using Frameworks;
using Microsoft.AspNetCore.Http;
using File = System.IO.File;

namespace UseCases;

public class StoreFile : IStoreFile
{
    private readonly IUnitOfWork _unitOfWork;

    public StoreFile(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public int Respond(IFormFile file)
    {
        // try
        // {
        //     if (file.Length > 0)
        //     {
        //         MemoryStream ms = new MemoryStream();
        //         file.CopyTo(ms);
        //
        //         var sha256 = SHA256.Create();
        //         var hash = sha256.ComputeHash(file.OpenReadStream());
        //
        //         var _file = _unitOfWork.Files.Add(new Entities.File()
        //         {
        //             Data = ms.ToArray(),
        //             Size = file.Length,
        //             UploadDate = DateTime.Now,
        //             HashCode = hash,
        //             Extension = Path.GetExtension(file.FileName)
        //         });
        //         _unitOfWork.Save();
        //         return _file.Id;
        //     }
        //     else
        //     {
        //         return 0;
        //     }
        // }
        // catch (Exception ex)
        // {
        //     return 0;
        // }
        return 1;
    }
}