using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Binary.Components;

[Serializable]
[NetSerializable]
public sealed class GasCanisterChangeReleasePressureMessage : BoundUserInterfaceMessage
{
	public float Pressure { get; }

	public GasCanisterChangeReleasePressureMessage(float pressure)
	{
		Pressure = pressure;
	}
}
