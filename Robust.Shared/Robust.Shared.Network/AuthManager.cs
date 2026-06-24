using System;
using System.Diagnostics.CodeAnalysis;

namespace Robust.Shared.Network;

internal sealed class AuthManager : IAuthManager
{
	public const string DefaultAuthServer = "https://auth.spacestation14.com/";

	public NetUserId? UserId { get; set; }

	public string? Server { get; set; } = "https://auth.spacestation14.com/";

	public string? Token { get; set; }

	public string? PubKey { get; set; }

	public bool AllowHwid { get; set; } = true;

	public void LoadFromEnv()
	{
		if (TryGetVar("ROBUST_AUTH_SERVER", out var val))
		{
			Server = val;
		}
		if (TryGetVar("ROBUST_AUTH_USERID", out var val2))
		{
			UserId = new NetUserId(Guid.Parse(val2));
		}
		if (TryGetVar("ROBUST_AUTH_PUBKEY", out var val3))
		{
			PubKey = val3;
		}
		if (TryGetVar("ROBUST_AUTH_TOKEN", out var val4))
		{
			Token = val4;
		}
		if (TryGetVar("ROBUST_AUTH_ALLOW_HWID", out var val5))
		{
			AllowHwid = val5.Trim() == "1";
		}
		static bool TryGetVar(string var, [NotNullWhen(true)] out string? reference)
		{
			reference = Environment.GetEnvironmentVariable(var);
			return reference != null;
		}
	}
}
