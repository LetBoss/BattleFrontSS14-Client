using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Turrets;

[Serializable]
[NetSerializable]
public enum DeployableTurretVisuals : byte
{
	Turret,
	Weapon,
	Broken
}
