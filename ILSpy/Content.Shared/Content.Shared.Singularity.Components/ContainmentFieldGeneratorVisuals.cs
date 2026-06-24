using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Singularity.Components;

[Serializable]
[NetSerializable]
public enum ContainmentFieldGeneratorVisuals : byte
{
	PowerLight,
	FieldLight,
	OnLight
}
