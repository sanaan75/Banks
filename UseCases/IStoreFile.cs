using Microsoft.AspNetCore.Http;

namespace UseCases;

public interface IStoreFile
{
    public int Respond(IFormFile file);
}