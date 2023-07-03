using Entities.Users;
using UseCases.Interfaces;

namespace Web;

public class GetUser : IGetUser
{
    private readonly IDb _db;

    public GetUser(IDb db)
    {
        _db = db;
    }

    public ProfileModel Get(int id)
    {
        var profile = _db.Query<User>().Select(i => new ProfileModel
        {
            Id = i.Id,
            Name = i.Username
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