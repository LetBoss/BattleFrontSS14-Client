using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.TacticalMap;

[Serializable]
[NetSerializable]
public sealed class TacticalMapUpdateCanvasMsg(List<TacticalMapLine> lines, Dictionary<Vector2i, string> labels) : BoundUserInterfaceMessage
{
	public readonly List<TacticalMapLine> Lines = lines;

	public readonly Dictionary<Vector2i, string> Labels = labels;
}
