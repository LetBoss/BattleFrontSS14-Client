using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Strip.Components;

[Serializable]
[NetSerializable]
public sealed class StrippingSlotButtonPressed(string slot, bool isHand) : BoundUserInterfaceMessage
{
	public readonly string Slot = slot;

	public readonly bool IsHand = isHand;
}
