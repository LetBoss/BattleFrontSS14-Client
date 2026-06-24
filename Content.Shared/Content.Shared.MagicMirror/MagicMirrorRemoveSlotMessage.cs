using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MagicMirror;

[Serializable]
[NetSerializable]
public sealed class MagicMirrorRemoveSlotMessage : BoundUserInterfaceMessage
{
	public MagicMirrorCategory Category { get; }

	public int Slot { get; }

	public MagicMirrorRemoveSlotMessage(MagicMirrorCategory category, int slot)
	{
		Category = category;
		Slot = slot;
	}
}
