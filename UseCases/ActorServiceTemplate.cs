using Entities;

namespace UseCases
{
    public abstract class ActorServiceTemplate : IActorService
    {
        public abstract void SetActor(Actor actor);

        public abstract Actor GetActor();

        public bool IsAuthenticated => GetActor()?.IsAuthenticated ?? false;

        public int UserId => GetActor().UserId;
        
    }
}
