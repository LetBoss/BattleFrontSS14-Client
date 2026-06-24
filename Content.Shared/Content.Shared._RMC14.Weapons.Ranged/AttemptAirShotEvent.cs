using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Weapons.Ranged;

[Serializable]
[ByRefEvent]
[NetSerializable]
public record struct AttemptAirShotEvent(NetEntity Shooter);
