using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Mortar;

[Serializable]
[NetSerializable]
public sealed class MortarTargetBuiMsg(Vector2i target) : BoundUserInterfaceMessage
{
	public Vector2i Target = target;
}
