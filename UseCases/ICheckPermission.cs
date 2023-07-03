using Entities.Permissions;

namespace UseCases;

public interface ICheckPermission
{
    public bool Respond(List<Permission> permissions, Permission permission);
}