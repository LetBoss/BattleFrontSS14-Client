using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Vehicle;

public readonly record struct VehicleMountedSlot(EntityUid Vehicle, EntityUid SlotOwner, string SlotId, VehicleSlotPath Path, string HardpointType, EntityUid? Item, EntityUid? ParentItem, VehicleSlotPath? ParentPath)
{
	public bool HasItem => Item.HasValue;

	public bool IsNested => ParentItem.HasValue;

	public string CompositeId => Path.ToCompositeId();

	public string? ParentSlotId => ParentPath?.ToCompositeId();
}
