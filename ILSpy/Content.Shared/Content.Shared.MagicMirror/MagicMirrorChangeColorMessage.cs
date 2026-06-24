using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.MagicMirror;

[Serializable]
[NetSerializable]
public sealed class MagicMirrorChangeColorMessage : BoundUserInterfaceMessage
{
	public MagicMirrorCategory Category { get; }

	public List<Color> Colors { get; }

	public int Slot { get; }

	public MagicMirrorChangeColorMessage(MagicMirrorCategory category, List<Color> colors, int slot)
	{
		Category = category;
		Colors = colors;
		Slot = slot;
	}
}
