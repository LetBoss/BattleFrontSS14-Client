using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MagicMirror;

[Serializable]
[NetSerializable]
public sealed class MagicMirrorAddSlotMessage : BoundUserInterfaceMessage
{
	public MagicMirrorCategory Category { get; }

	public MagicMirrorAddSlotMessage(MagicMirrorCategory category)
	{
		Category = category;
	}
}
