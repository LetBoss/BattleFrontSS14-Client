using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Morgue;

[Serializable]
[NetSerializable]
public enum MorgueContents : byte
{
	Empty,
	HasMob,
	HasSoul,
	HasContents
}
