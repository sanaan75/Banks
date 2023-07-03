using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Entities;
using Entities.Users;
using Entities.Utilities;
using Entities.Utilities.Exceptions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UseCases.Interfaces;

namespace Web.Jwt;

public class JwtAuth : IJwtAuth
{
    private readonly IDb _db;
    private readonly byte[] _encKey;
    private readonly byte[] _key;
    private readonly IOptions<JwtAuthConf> _options;

    public JwtAuth(IDb db, IOptions<JwtAuthConf> options)
    {
        _db = db;
        _options = options;
        _key = AppSetting.JwtKey;
        _encKey = AppSetting.JwtEncryptionKey;
    }

    public string Authentication(string username)
    {
        var tokenDetail = SetTokenDetails(username);

        DeactivateOtherTokens(tokenDetail.UserId);

        var state = tokenDetail.State;

        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                new[]
                {
                    new(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Expiration, DateTime.UtcNow.AddHours(72).ToString()),
                    new Claim(AppClaimTypes.State, state)
                }),
            Expires = DateTime.UtcNow.AddHours(72),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature),
            EncryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(_encKey),
                SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes256CbcHmacSha512)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public bool Logout(string email, string jwtState)
    {
        var user = _db.Query<User>().GetByUsername(email);

        if (user is null)
            throw new AppException(404, "user with that username was not found");

        var tokenDetail = _db.Query<TokenDetail>()
                              .FirstOrDefault(detail => detail.State.Equals(jwtState) && detail.UserId == user.Id) ??
                          throw new AppException(404, "state was not found");

        tokenDetail.IsActive = false;
        UpdateTokenState(tokenDetail);

        _db.Save();

        return true;
    }

    private TokenDetail SetTokenDetails(string username)
    {
        var query = _db.Query<User>();
        var user = query.GetByUsername(username);

        if (user is null)
            throw new AppException(404, "Username Was Not Found");

        var tokenDetail = new TokenDetail
        {
            UserId = user.Id,
            Expire = DateTime.UtcNow.AddHours(72),
            IsActive = true
        };

        UpdateTokenState(tokenDetail);

        _db.Set<TokenDetail>().Add(tokenDetail);
        _db.Save();
        return tokenDetail;
    }


    //our query must be OrderByDescending in order to Deactivating older tokens work properly
    private void DeactivateOtherTokens(int? userId)
    {
        //_db.Query returns an OrderByDescending query
        var userTokenDetails = _db.Query<TokenDetail>()
            .Filter(userId, tokenDetail => tokenDetail.UserId == userId && tokenDetail.IsActive)
            .ToList();
        for (var i = 0; i < userTokenDetails.Count; i++)
        {
            if (i < _options.Value.AllowUserAccountNo)
                continue;
            userTokenDetails[i].IsActive = false;
        }

        _db.Save();
    }


    private void UpdateTokenState(TokenDetail tokenDetail)
    {
        tokenDetail.State = Guid.NewGuid().ToString().Replace("-", "");
    }
}