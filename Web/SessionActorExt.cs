using System.Text.Json;
using Entities;
using Microsoft.AspNetCore.Http;

namespace Web;

public static class SessionActorExt
{
    private const string ActorSessionKey = "DefAppKey";
    public static Actor _actor = new() {IsAuthenticated = false};

    public static void SetActor(this ISession session, Actor actor)
    {
        session.SetString(ActorSessionKey, JsonSerializer.Serialize(actor));
        _actor = actor;
    }

    public static Actor GetActor(this ISession session)
    {
        var value = session.GetString(ActorSessionKey);
        if (string.IsNullOrEmpty(value))
            return new Actor {IsAuthenticated = false};

        _actor = JsonSerializer.Deserialize<Actor>(value);
        return _actor;
    }

    public static void LogoutActor(this ISession session)
    {
        session.Remove(ActorSessionKey);
    }
}