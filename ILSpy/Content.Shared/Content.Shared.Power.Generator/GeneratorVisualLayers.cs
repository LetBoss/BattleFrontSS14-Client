using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Power.Generator;

[Serializable]
[NetSerializable]
public enum GeneratorVisualLayers : byte
{
	Body,
	Unlit
}
