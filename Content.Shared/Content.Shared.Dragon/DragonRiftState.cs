using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Dragon;

[Serializable]
[NetSerializable]
public enum DragonRiftState : byte
{
	Charging,
	AlmostFinished,
	Finished
}
