using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._RMC14.Explosion;

[Serializable]
[DataRecord]
[NetSerializable]
public record struct Hit(NetEntity Id, [field: DataField(null, false, 1, false, false, typeof(TimeOffsetSerializer))] TimeSpan ExpireAt, int? ExtraId);
