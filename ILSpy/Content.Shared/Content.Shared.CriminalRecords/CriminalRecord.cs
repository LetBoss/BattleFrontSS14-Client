using System;
using System.Collections.Generic;
using Content.Shared.Security;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.CriminalRecords;

[Serializable]
[NetSerializable]
[DataRecord]
public sealed record CriminalRecord
{
	[DataField(null, false, 1, false, false, null)]
	public SecurityStatus Status;

	[DataField(null, false, 1, false, false, null)]
	public string? Reason;

	[DataField(null, false, 1, false, false, null)]
	public string? InitiatorName;

	[DataField(null, false, 1, false, false, null)]
	public List<CrimeHistory> History = new List<CrimeHistory>();
}
