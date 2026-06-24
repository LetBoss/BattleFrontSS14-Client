using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Turrets;

[Serializable]
[NetSerializable]
public enum DeployableTurretState : byte
{
	Retracted = 0,
	Deployed = 1,
	Retracting = 2,
	Deploying = 3,
	Firing = 5,
	Disabled = 8,
	Broken = 16
}
