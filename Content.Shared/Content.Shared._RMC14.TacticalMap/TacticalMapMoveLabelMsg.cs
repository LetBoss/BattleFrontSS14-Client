using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.TacticalMap;

[Serializable]
[NetSerializable]
public sealed class TacticalMapMoveLabelMsg(Vector2i oldPosition, Vector2i newPosition) : BoundUserInterfaceMessage
{
	public readonly Vector2i OldPosition = oldPosition;

	public readonly Vector2i NewPosition = newPosition;
}
