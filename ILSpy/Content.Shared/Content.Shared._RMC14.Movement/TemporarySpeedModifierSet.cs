using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Movement;

[Serializable]
[DataRecord]
[NetSerializable]
public record struct TemporarySpeedModifierSet(TimeSpan Duration, float Walk, float Sprint);
