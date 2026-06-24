using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Trinary.Components;

[Serializable]
[NetSerializable]
public sealed class GasMixerChangeOutputPressureMessage : BoundUserInterfaceMessage
{
	public float Pressure { get; }

	public GasMixerChangeOutputPressureMessage(float pressure)
	{
		Pressure = pressure;
	}
}
