using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Binary.Components;

[Serializable]
[NetSerializable]
public sealed class GasPressureRegulatorChangeThresholdMessage(float pressure) : BoundUserInterfaceMessage
{
	public float ThresholdPressure { get; } = pressure;
}
