using Robust.Shared.GameObjects;

namespace Content.Shared._PUBG;

public sealed class ZonePhaseChangedEvent : EntityEventArgs
{
	public int NewPhase { get; }

	public ZonePhaseChangedEvent(int newPhase)
	{
		NewPhase = newPhase;
	}
}
