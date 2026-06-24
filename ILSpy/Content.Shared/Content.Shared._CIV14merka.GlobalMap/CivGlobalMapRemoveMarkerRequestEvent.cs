using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.GlobalMap;

[Serializable]
[NetSerializable]
public sealed class CivGlobalMapRemoveMarkerRequestEvent : EntityEventArgs
{
	public int MarkerId { get; }

	public CivGlobalMapRemoveMarkerRequestEvent(int markerId)
	{
		MarkerId = markerId;
	}
}
