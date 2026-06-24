using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Crayon;

[Serializable]
[NetSerializable]
public sealed class CrayonColorMessage : BoundUserInterfaceMessage
{
	public readonly Color Color;

	public CrayonColorMessage(Color color)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Color = color;
	}
}
