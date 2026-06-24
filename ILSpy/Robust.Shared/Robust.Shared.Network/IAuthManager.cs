namespace Robust.Shared.Network;

internal interface IAuthManager
{
	NetUserId? UserId { get; set; }

	string? Server { get; set; }

	string? Token { get; set; }

	string? PubKey { get; set; }

	bool AllowHwid { get; set; }

	void LoadFromEnv();
}
