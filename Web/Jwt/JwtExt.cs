using System.Net.Mime;
using System.Security.Claims;
using Entities;
using Entities.Users;
using Entities.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using UseCases.Interfaces;

namespace Web.Jwt;

public static class JwtExt
{
    public static AuthenticationBuilder AddJwtServices(this IServiceCollection services,
        Action<JwtAuthConf>? option = null)
    {
        if (option is not null) services.Configure(option);

        var message = "";
        return services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    RequireSignedTokens = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(AppSetting.JwtKey),
                    TokenDecryptionKey =
                        new SymmetricSecurityKey(AppSetting.JwtEncryptionKey),
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = ctx =>
                    {
                        ctx.Response.StatusCode = StatusCodes.Status302Found;
                        message += "Authentication Failed";
                        return Task.CompletedTask;
                    },

                    OnChallenge = ctx =>
                    {
                        ctx.HandleResponse();
                        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        ctx.Response.ContentType = MediaTypeNames.Application.Json;
                        message = string.IsNullOrWhiteSpace(message)
                            ? "Authentication Failed Please Check Your Token"
                            : message;
                        return ctx.Response.WriteAsync(new { message }.ToJson());
                    },

                    OnMessageReceived = ctx =>
                    {
                        message = "";
                        ctx.Request.Headers.TryGetValue("Authorization", out var bearerToken);
                        if (bearerToken.Count == 0)
                            message += "No Bearer Token Sent";
                        return Task.CompletedTask;
                    },

                    OnTokenValidated = context =>
                    {
                        var claims = context.Principal!.Claims;
                        var currentUserEmail = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name).Value;
                        var currentUserState = claims.FirstOrDefault(claim => claim.Type == AppClaimTypes.State)?.Value;
                        var db = services.BuildServiceProvider().GetService<IDb>();

                        var user = db.Query<User>().GetByUsername(currentUserEmail);
                        if (currentUserState is null)
                        {
                            context.Fail("Token Is Invalid");
                            message += "Token Is Invalid";
                            return Task.CompletedTask;
                        }

                        var tokenDetailQuery = db.Query<TokenDetail>().Where(x => x.UserId == user.Id);

                        var isTokenActive =
                            tokenDetailQuery.FirstOrDefault(detail => detail.State.Equals(currentUserState))!.IsActive;
                        if (!isTokenActive)
                        {
                            context.Fail("Token Is Not Active");
                            message += "Token Is Not Active";
                        }

                        var isStateValid = tokenDetailQuery.Any(x => x.State.Equals(currentUserState));
                        if (!isStateValid)
                        {
                            context.Fail("Token Is Invalid");
                            message += "Token Is Invalid";
                            return Task.CompletedTask;
                        }

                        return Task.CompletedTask;
                    }
                };
            });
    }
}