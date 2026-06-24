using Robust.Shared.GameObjects;
using Robust.Shared.Player;

namespace Robust.Shared.GameStates;

[ByRefEvent]
[ComponentEvent]
public struct ComponentGetStateAttemptEvent(ICommonSession? player)
{
	public readonly ICommonSession? Player = player;

	public bool Cancelled = false;
}
