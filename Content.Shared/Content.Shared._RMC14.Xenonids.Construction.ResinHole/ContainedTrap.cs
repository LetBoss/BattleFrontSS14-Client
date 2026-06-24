using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids.Construction.ResinHole;

[Serializable]
[NetSerializable]
public enum ContainedTrap
{
	Empty,
	Parasite,
	NeuroticGas,
	AcidGas,
	Acid1,
	Acid2,
	Acid3
}
