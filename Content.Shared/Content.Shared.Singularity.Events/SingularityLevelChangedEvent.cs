using Content.Shared.Singularity.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Singularity.Events;

public sealed class SingularityLevelChangedEvent : EntityEventArgs
{
	public readonly byte NewValue;

	public readonly byte OldValue;

	public readonly SingularityComponent Singularity;

	public SingularityLevelChangedEvent(byte newValue, byte oldValue, SingularityComponent singularity)
	{
		NewValue = newValue;
		OldValue = oldValue;
		Singularity = singularity;
	}
}
