using System.Diagnostics.CodeAnalysis;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Robust.Shared.GameStates;

[ByRefEvent]
[ComponentEvent]
public struct ComponentGetState
{
	public readonly ICommonSession? Player;

	public GameTick FromTick { get; }

	public IComponentState? State { get; set; }

	[MemberNotNullWhen(false, "Player")]
	public bool ReplayState
	{
		[MemberNotNullWhen(false, "Player")]
		get
		{
			return Player == null;
		}
	}

	public ComponentGetState(ICommonSession? player, GameTick fromTick)
	{
		Player = player;
		FromTick = fromTick;
		State = null;
	}
}
