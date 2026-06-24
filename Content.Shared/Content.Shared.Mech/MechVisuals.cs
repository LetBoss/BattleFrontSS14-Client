using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Mech;

[Serializable]
[NetSerializable]
public enum MechVisuals : byte
{
	Open,
	Broken
}
