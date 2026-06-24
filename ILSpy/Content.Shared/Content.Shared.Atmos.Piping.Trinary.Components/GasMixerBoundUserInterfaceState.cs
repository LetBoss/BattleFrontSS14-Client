using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Trinary.Components;

[Serializable]
[NetSerializable]
public sealed class GasMixerBoundUserInterfaceState : BoundUserInterfaceState
{
	public string MixerLabel { get; }

	public float OutputPressure { get; }

	public bool Enabled { get; }

	public float NodeOne { get; }

	public GasMixerBoundUserInterfaceState(string mixerLabel, float outputPressure, bool enabled, float nodeOne)
	{
		MixerLabel = mixerLabel;
		OutputPressure = outputPressure;
		Enabled = enabled;
		NodeOne = nodeOne;
	}
}
