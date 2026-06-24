using System;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Projectiles;

[Serializable]
[DataRecord]
[NetSerializable]
public record struct DamageFalloffThreshold(float Range, FixedPoint2 Falloff, bool IgnoreModifiers);
