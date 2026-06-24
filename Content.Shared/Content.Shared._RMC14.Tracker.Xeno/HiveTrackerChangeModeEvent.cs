using System;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Tracker.Xeno;

[Serializable]
[DataRecord]
[NetSerializable]
public sealed record HiveTrackerChangeModeEvent(ProtoId<TrackerModePrototype> Mode);
