using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Loadout;

[Serializable]
[NetSerializable]
public enum PubgWeaponModuleVisuals : byte
{
	Optic,
	Muzzle,
	Magazine
}
