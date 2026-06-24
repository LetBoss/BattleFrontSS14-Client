using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MagicMirror;

[Serializable]
[NetSerializable]
public sealed class MagicMirrorSelectMessage : BoundUserInterfaceMessage
{
	public MagicMirrorCategory Category { get; }

	public string Marking { get; }

	public int Slot { get; }

	public MagicMirrorSelectMessage(MagicMirrorCategory category, string marking, int slot)
	{
		Category = category;
		Marking = marking;
		Slot = slot;
	}
}
