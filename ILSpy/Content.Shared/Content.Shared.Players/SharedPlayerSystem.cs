using Robust.Shared.GameObjects;
using Robust.Shared.Player;

namespace Content.Shared.Players;

public abstract class SharedPlayerSystem : EntitySystem
{
	public abstract ContentPlayerData? ContentData(ICommonSession? session);
}
