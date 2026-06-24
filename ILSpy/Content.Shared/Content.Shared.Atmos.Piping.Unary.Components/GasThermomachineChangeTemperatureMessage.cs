using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Unary.Components;

[Serializable]
[NetSerializable]
public sealed class GasThermomachineChangeTemperatureMessage : BoundUserInterfaceMessage
{
	public float Temperature { get; }

	public GasThermomachineChangeTemperatureMessage(float temperature)
	{
		Temperature = temperature;
	}
}
