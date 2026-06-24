using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Portable.Components;

[Serializable]
[NetSerializable]
public sealed class SpaceHeaterChangeTemperatureMessage : BoundUserInterfaceMessage
{
	public float Temperature { get; }

	public SpaceHeaterChangeTemperatureMessage(float temperature)
	{
		Temperature = temperature;
	}
}
