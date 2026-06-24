using Robust.Shared.Network;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Player;

public sealed class SessionData
{
	[ViewVariables]
	public NetUserId UserId { get; }

	[ViewVariables]
	public string UserName { get; }

	[ViewVariables]
	public object? ContentDataUncast { get; set; }

	public SessionData(NetUserId userId, string userName)
	{
		UserId = userId;
		UserName = userName;
	}
}
