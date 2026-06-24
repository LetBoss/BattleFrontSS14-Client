using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Binary.Components;

[Serializable]
[NetSerializable]
public sealed class GasPressurePumpChangeOutputPressureMessage(float pressure) : BoundUserInterfaceMessage
{
	public float Pressure { get; } = pressure;
}
