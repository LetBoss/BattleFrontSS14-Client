using System;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Robust.Shared.GameObjects;

[Serializable]
[NetSerializable]
public readonly struct ComponentChange(ushort netId, IComponentState? state, GameTick lastModifiedTick)
{
	public readonly IComponentState? State = state;

	public readonly ushort NetID = netId;

	public readonly GameTick LastModifiedTick = lastModifiedTick;

	public override string ToString()
	{
		return $"{NetID} {State?.GetType().Name}";
	}
}
