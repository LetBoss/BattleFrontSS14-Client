using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Intel;

[Serializable]
[NetSerializable]
[DataRecord]
public record struct IntelObjectiveAmount(int Current, int Total);
