using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Console;

[Serializable]
[NetSerializable]
public sealed class PubgWeaponVendingDispenseMessage : BoundUserInterfaceMessage
{
	public string ItemId { get; }

	public PubgWeaponVendingDispenseMessage(string itemId)
	{
		ItemId = itemId;
	}
}
