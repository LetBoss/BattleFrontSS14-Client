using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Singularity.Components;

[Serializable]
[NetSerializable]
public enum FieldLevelVisuals : byte
{
	NoLevel,
	On,
	OneField,
	MultipleFields
}
