using Entities.Permissions;

namespace UseCases;

public class CheckPermission : ICheckPermission
{
    private readonly IActorService _actorService;

    public CheckPermission(IActorService actorService)
    {
        _actorService = actorService;
    }

    public bool Respond(List<Permission> permissions, Permission permission)
    {
        var actor = _actorService.GetActor();
        if (actor.IsSysAdmin)
            return true;

        return permissions.Contains(permission);
    }
}