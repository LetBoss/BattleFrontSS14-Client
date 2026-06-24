using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Crayon;

[Serializable]
[NetSerializable]
public sealed class CrayonBoundUserInterfaceState : BoundUserInterfaceState
{
	public string Selected;

	public bool SelectableColor;

	public Color Color;

	public CrayonBoundUserInterfaceState(string selected, bool selectableColor, Color color)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Selected = selected;
		SelectableColor = selectableColor;
		Color = color;
	}
}
