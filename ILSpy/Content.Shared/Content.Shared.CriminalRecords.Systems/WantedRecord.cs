using System;
using System.Collections.Generic;
using Content.Shared.Security;
using Content.Shared.StationRecords;
using Robust.Shared.Serialization;

namespace Content.Shared.CriminalRecords.Systems;

[Serializable]
[NetSerializable]
public struct WantedRecord(GeneralStationRecord targetInfo, SecurityStatus status, string? reason, string? initiator, List<CrimeHistory> history)
{
	public GeneralStationRecord TargetInfo = targetInfo;

	public SecurityStatus Status = status;

	public string? Reason = reason;

	public string? Initiator = initiator;

	public List<CrimeHistory> History = history;
}
