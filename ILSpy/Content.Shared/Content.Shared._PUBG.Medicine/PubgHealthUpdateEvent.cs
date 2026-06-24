using System;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Medicine;

[Serializable]
[NetSerializable]
public sealed class PubgHealthUpdateEvent : EntityEventArgs
{
	public FixedPoint2 CurrentHp { get; }

	public FixedPoint2 MaxHp { get; }

	public PubgHealthUpdateEvent(FixedPoint2 currentHp, FixedPoint2 maxHp)
	{
		CurrentHp = currentHp;
		MaxHp = maxHp;
	}
}
