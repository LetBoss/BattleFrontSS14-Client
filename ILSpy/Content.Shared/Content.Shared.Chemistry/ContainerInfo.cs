using System;
using System.Collections.Generic;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry;

[Serializable]
[NetSerializable]
public sealed class ContainerInfo
{
	public readonly string DisplayName;

	public readonly FixedPoint2 CurrentVolume;

	public readonly FixedPoint2 MaxVolume;

	public List<(string Id, FixedPoint2 Quantity)>? Entities { get; init; }

	public List<ReagentQuantity>? Reagents { get; init; }

	public ContainerInfo(string displayName, FixedPoint2 currentVolume, FixedPoint2 maxVolume)
	{
		DisplayName = displayName;
		CurrentVolume = currentVolume;
		MaxVolume = maxVolume;
	}
}
