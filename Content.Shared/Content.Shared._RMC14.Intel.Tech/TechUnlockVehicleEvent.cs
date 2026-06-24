using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Intel.Tech;

[Serializable]
[DataRecord]
[NetSerializable]
public sealed record TechUnlockVehicleEvent(string Unlock);
