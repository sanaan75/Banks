using Entities;

namespace UseCases;

public interface ILogin
{
    public Actor Respond(string username, string password);

    public Actor GetActor();
}