using Robust.Shared.GameObjects;

namespace Content.Shared.PowerCell.Components;

public sealed class PowerCellChangedEvent : EntityEventArgs
{
	public readonly bool Ejected;

	public PowerCellChangedEvent(bool ejected)
	{
		Ejected = ejected;
	}
}
