using System;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Clothing.Components;

[Serializable]
[NetSerializable]
public sealed class ChameleonBoundUserInterfaceState : BoundUserInterfaceState
{
	public readonly SlotFlags Slot;

	public readonly string? SelectedId;

	public readonly string? RequiredTag;

	public ChameleonBoundUserInterfaceState(SlotFlags slot, string? selectedId, string? requiredTag)
	{
		Slot = slot;
		SelectedId = selectedId;
		RequiredTag = requiredTag;
	}
}
