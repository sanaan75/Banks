using Framework;

namespace Web;

public class GetUser : IGetUser
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUser(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public ProfileModel Get(int id)
    {
        var profile = _unitOfWork.Users.GetAll().Select(i => new ProfileModel
        {
            Id = i.Id,
            Name = i.Username,
        }).FirstOrDefault(i => i.Id == id);
        if (profile.Picture == null)
            profile.Picture = "/lib/img/user.png";

        return profile;
    }
}
public class ProfileModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Picture { get; set; }
}