using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Light.Components;

[Serializable]
[NetSerializable]
public sealed class RgbLightControllerState : ComponentState
{
	public readonly float CycleRate;

	public List<int>? Layers;

	public RgbLightControllerState(float cycleRate, List<int>? layers)
	{
		CycleRate = cycleRate;
		Layers = layers;
	}
}
