using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Stun;

[Serializable]
[NetSerializable]
public enum RMCSizes : byte
{
	Small,
	Humanoid,
	VerySmallXeno,
	SmallXeno,
	Xeno,
	Big,
	Immobile
}
