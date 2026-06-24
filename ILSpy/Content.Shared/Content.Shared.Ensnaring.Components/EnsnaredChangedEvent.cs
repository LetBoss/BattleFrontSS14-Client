using Robust.Shared.GameObjects;

namespace Content.Shared.Ensnaring.Components;

public sealed class EnsnaredChangedEvent : EntityEventArgs
{
	public readonly bool IsEnsnared;

	public EnsnaredChangedEvent(bool isEnsnared)
	{
		IsEnsnared = isEnsnared;
	}
}
