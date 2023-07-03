using Entities;

namespace UseCases;

public interface IActorService
{
    bool IsAuthenticated { get; }
    int UserId { get; }
    void SetActor(Actor actor);
    Actor GetActor();
}