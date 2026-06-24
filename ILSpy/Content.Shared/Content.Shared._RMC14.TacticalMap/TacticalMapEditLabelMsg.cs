using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.TacticalMap;

[Serializable]
[NetSerializable]
public sealed class TacticalMapEditLabelMsg(Vector2i position, string newText) : BoundUserInterfaceMessage
{
	public readonly Vector2i Position = position;

	public readonly string NewText = newText;
}
