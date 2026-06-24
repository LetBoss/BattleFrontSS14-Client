using System;
using System.Collections.Generic;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.GameTicking;

[Serializable]
[NetSerializable]
public sealed class TickerJobsAvailableEvent(Dictionary<NetEntity, string> stationNames, Dictionary<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>> jobsAvailableByStation) : EntityEventArgs
{
	public Dictionary<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>> JobsAvailableByStation { get; } = jobsAvailableByStation;

	public Dictionary<NetEntity, string> StationNames { get; } = stationNames;
}
