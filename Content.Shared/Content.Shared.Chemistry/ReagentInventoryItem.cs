using System;
using Content.Shared.FixedPoint;
using Content.Shared.Storage;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry;

[Serializable]
[NetSerializable]
public sealed class ReagentInventoryItem(ItemStorageLocation storageLocation, string reagentLabel, FixedPoint2 quantity, Color reagentColor)
{
	public ItemStorageLocation StorageLocation = storageLocation;

	public string ReagentLabel = reagentLabel;

	public FixedPoint2 Quantity = quantity;

	public Color ReagentColor = reagentColor;
}
