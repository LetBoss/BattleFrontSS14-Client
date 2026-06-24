using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Pinpointer;

[Serializable]
[NetSerializable]
public sealed class NavMapBeaconConfigureBuiMessage : BoundUserInterfaceMessage
{
	public string? Text;

	public bool Enabled;

	public Color Color;

	public NavMapBeaconConfigureBuiMessage(string? text, bool enabled, Color color)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Text = text;
		Enabled = enabled;
		Color = color;
	}
}
