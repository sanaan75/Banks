using Entities;
using UseCases;

namespace Web;

public class ActorService : ActorServiceTemplate
{
    private static Actor _actor;

    public override void SetActor(Actor actor)
    {
        _actor = actor;
    }

    public override Actor GetActor()
    {
        if (_actor.IsAuthenticated)
            return _actor;

        if (SessionActorExt._actor.IsAuthenticated == false)
            return new Actor {IsAuthenticated = false};

        return SessionActorExt._actor;
    }
}