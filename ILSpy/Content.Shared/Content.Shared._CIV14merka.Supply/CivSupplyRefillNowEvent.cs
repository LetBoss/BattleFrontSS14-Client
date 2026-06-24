using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Supply;

[Serializable]
[NetSerializable]
public sealed class CivSupplyRefillNowEvent : EntityEventArgs
{
	public string ProtoId;

	public int Amount;

	public CivSupplyRefillNowEvent(string protoId, int amount)
	{
		ProtoId = protoId;
		Amount = amount;
	}
}
