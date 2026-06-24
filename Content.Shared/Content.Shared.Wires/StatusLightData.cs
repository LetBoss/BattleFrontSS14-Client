using System;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Wires;

[Serializable]
[NetSerializable]
public struct StatusLightData(Color color, StatusLightState state, string text)
{
	public Color Color { get; } = color;

	public StatusLightState State { get; } = state;

	public string Text { get; } = text;

	public override string ToString()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		return $"Color: {Color}, State: {State}, Text: {Text}";
	}
}
