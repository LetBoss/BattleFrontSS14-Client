using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Power.Generation.Teg;

[Serializable]
[NetSerializable]
public enum TegVisualLayers
{
	PowerOutput,
	CirculatorBase,
	CirculatorLight
}
