using System;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Tracker.SquadLeader;

[Serializable]
[DataRecord]
[NetSerializable]
public sealed record SquadLeaderTrackerChangeModeEvent(ProtoId<TrackerModePrototype> Mode);
