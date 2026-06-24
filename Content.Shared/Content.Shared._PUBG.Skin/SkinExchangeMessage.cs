using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Skin;

[Serializable]
[NetSerializable]
public sealed class SkinExchangeMessage : EntityEventArgs
{
	public int Amount { get; }

	public SkinExchangeMessage(int amount)
	{
		Amount = amount;
	}
}
