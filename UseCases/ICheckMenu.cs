using Entities.Permissions;

namespace UseCases;

public interface ICheckMenu
{
    public string Respond(Menu selected);
}