using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivRespawnChoiceEvent : EntityEventArgs
{
	public int RespawnId { get; }

	public bool Accept { get; }

	public CivRespawnChoiceEvent(int respawnId, bool accept)
	{
		RespawnId = respawnId;
		Accept = accept;
	}
}
