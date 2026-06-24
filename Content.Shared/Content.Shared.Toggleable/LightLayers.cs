using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Toggleable;

[Serializable]
[NetSerializable]
public enum LightLayers : byte
{
	Light,
	Unshaded
}
