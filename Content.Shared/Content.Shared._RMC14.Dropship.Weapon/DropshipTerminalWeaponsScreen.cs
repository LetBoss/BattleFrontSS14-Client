using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Dropship.Weapon;

[Serializable]
[NetSerializable]
public enum DropshipTerminalWeaponsScreen
{
	Main,
	Equip,
	Target,
	Strike,
	StrikeWeapon,
	Cams,
	SelectingWeapon,
	Medevac,
	Fulton,
	Paradrop,
	Spotlight,
	TacMap,
	EquipmentDeployer
}
