using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Skin;

[Serializable]
[NetSerializable]
public sealed class SkinSellMessage : EntityEventArgs
{
	public string ItemId { get; }

	public SkinSellMessage(string itemId)
	{
		ItemId = itemId;
	}
}
