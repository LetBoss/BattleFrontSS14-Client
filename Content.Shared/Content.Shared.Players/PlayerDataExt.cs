using Robust.Shared.GameObjects;
using Robust.Shared.Player;

namespace Content.Shared.Players;

public static class PlayerDataExt
{
	public static ContentPlayerData? ContentData(this SessionData data)
	{
		return (ContentPlayerData)data.ContentDataUncast;
	}

	public static ContentPlayerData? ContentData(this ICommonSession session)
	{
		return session.Data.ContentData();
	}

	public static EntityUid? GetMind(this ICommonSession session)
	{
		return session.Data.ContentData()?.Mind;
	}
}
