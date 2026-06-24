using System;
using System.Collections.Generic;
using Content.Shared.Security;
using Content.Shared.StationRecords;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CriminalRecords;

[Serializable]
[NetSerializable]
public sealed class CriminalRecordsConsoleState : BoundUserInterfaceState
{
	public uint? SelectedKey;

	public CriminalRecord? CriminalRecord;

	public GeneralStationRecord? StationRecord;

	public SecurityStatus FilterStatus;

	public readonly Dictionary<uint, string>? RecordListing;

	public readonly StationRecordsFilter? Filter;

	public CriminalRecordsConsoleState(Dictionary<uint, string>? recordListing, StationRecordsFilter? newFilter)
	{
		RecordListing = recordListing;
		Filter = newFilter;
	}

	public CriminalRecordsConsoleState()
		: this(null, null)
	{
	}

	public bool IsEmpty()
	{
		if (!SelectedKey.HasValue && StationRecord == null && CriminalRecord == null)
		{
			return RecordListing == null;
		}
		return false;
	}
}
