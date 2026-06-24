using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Mortar;

[Serializable]
[NetSerializable]
public sealed class CivMortarTargetBuiMsg(Vector2i target) : BoundUserInterfaceMessage
{
	public Vector2i Target = target;
}
