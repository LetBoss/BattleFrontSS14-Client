using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Supply;

[Serializable]
[NetSerializable]
public sealed class CivSupplySetPeriodicEvent : EntityEventArgs
{
	public string ProtoId;

	public int Amount;

	public CivSupplySetPeriodicEvent(string protoId, int amount)
	{
		ProtoId = protoId;
		Amount = amount;
	}
}
