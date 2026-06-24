using Robust.Shared.Player;

namespace Content.Shared.Players.RateLimiting;

public abstract class SharedPlayerRateLimitManager
{
	public abstract RateLimitStatus CountAction(ICommonSession player, string key);

	public abstract void Register(string key, RateLimitRegistration registration);

	public abstract void Initialize();
}
