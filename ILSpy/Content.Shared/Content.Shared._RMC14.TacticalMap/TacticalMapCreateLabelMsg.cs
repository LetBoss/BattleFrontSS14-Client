using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.TacticalMap;

[Serializable]
[NetSerializable]
public sealed class TacticalMapCreateLabelMsg(Vector2i position, string text) : BoundUserInterfaceMessage
{
	public readonly Vector2i Position = position;

	public readonly string Text = text;
}
