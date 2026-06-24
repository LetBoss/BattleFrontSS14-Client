using System;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._RMC14.Medical.Wounds;

[Serializable]
[DataRecord]
[NetSerializable]
public record struct Wound(FixedPoint2 Damage, FixedPoint2 Healed, float Bloodloss, [field: DataField(null, false, 1, false, false, typeof(TimeOffsetSerializer))] TimeSpan? StopBleedAt, WoundType Type, bool Treated);
