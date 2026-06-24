using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Light.Components;

[Serializable]
[NetSerializable]
public enum LightBulbState : byte
{
	Normal,
	Broken,
	Burned
}
