using System;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Roof;

[Serializable]
[NetSerializable]
public enum CivRoofStage : byte
{
	Intact,
	Cracked,
	Broken
}
