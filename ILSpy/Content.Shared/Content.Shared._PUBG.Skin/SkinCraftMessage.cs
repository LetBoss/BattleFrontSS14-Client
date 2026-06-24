using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Skin;

[Serializable]
[NetSerializable]
public sealed class SkinCraftMessage : EntityEventArgs
{
	public string ItemId { get; }

	public SkinCraftMessage(string itemId)
	{
		ItemId = itemId;
	}
}
