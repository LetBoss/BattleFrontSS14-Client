using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Power;

[Serializable]
[DataRecord]
[NetSerializable]
public record struct RMCApcChannel(RMCApcButtonState Button, int Watts, bool On);
