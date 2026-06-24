using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Weapons.Ranged.Ammo.BulletBox;

[Serializable]
[NetSerializable]
public enum BulletBoxVisuals
{
	Empty,
	Low,
	Medium,
	High,
	Full
}
