using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Trinary.Components;

[Serializable]
[NetSerializable]
public sealed class GasFilterChangeRateMessage : BoundUserInterfaceMessage
{
	public float Rate { get; }

	public GasFilterChangeRateMessage(float rate)
	{
		Rate = rate;
	}
}
