using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Power.Generator;

[Serializable]
[NetSerializable]
public enum GeneratorVisuals : byte
{
	Running
}
