using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Ranged.Systems;

[Serializable]
[NetSerializable]
public enum AmmoVisuals : byte
{
	Spent,
	AmmoCount,
	AmmoMax,
	HasAmmo,
	MagLoaded,
	BoltClosed
}
