namespace Web.Jwt;

public interface IJwtAuth
{
    string Authentication(string email);
    bool Logout(string email, string jwtState);
}