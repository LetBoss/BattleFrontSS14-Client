using Content.Shared.Players.RateLimiting;
using Robust.Shared.Player;

namespace Content.Client.Players.RateLimiting;

public sealed class PlayerRateLimitManager : SharedPlayerRateLimitManager
{
	public override RateLimitStatus CountAction(ICommonSession player, string key)
	{
		return RateLimitStatus.Allowed;
	}

	public override void Register(string key, RateLimitRegistration registration)
	{
	}

	public override void Initialize()
	{
	}
}
