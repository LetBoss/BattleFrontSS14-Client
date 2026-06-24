using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Light;

[Serializable]
[NetSerializable]
public enum PoweredLightState : byte
{
	Empty,
	On,
	Off,
	Broken,
	Burned
}
