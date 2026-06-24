using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.OrbitalCannon;

[Serializable]
[NetSerializable]
public enum OrbitalCannonStatus
{
	Unloaded,
	Loaded,
	Chambered
}
