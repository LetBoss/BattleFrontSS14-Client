using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Emplacements;

[Serializable]
[NetSerializable]
public enum WeaponMountComponentVisualLayers : byte
{
	Mounted,
	MountedAmmo,
	Folded,
	FoldedAmmo,
	Broken,
	Overheated
}
