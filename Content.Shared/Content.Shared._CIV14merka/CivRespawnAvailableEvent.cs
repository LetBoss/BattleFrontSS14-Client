using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivRespawnAvailableEvent : EntityEventArgs
{
	public int RespawnId { get; }

	public CivRespawnAvailableEvent(int respawnId)
	{
		RespawnId = respawnId;
	}
}
