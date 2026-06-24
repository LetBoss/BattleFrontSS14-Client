using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Weapons.Ranged.Laser;

[Serializable]
[DataRecord]
[NetSerializable]
public readonly record struct GunToggleableLaserSetting(Rsi Icon);
