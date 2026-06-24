using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Electrocution;

[Serializable]
[NetSerializable]
public enum ElectrifiedLayers : byte
{
	Sparks,
	HUD
}
