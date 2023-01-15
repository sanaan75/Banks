using Web.Models;

namespace Web;

public interface IGetUser
{
    public ProfileModel Get(int id);
}