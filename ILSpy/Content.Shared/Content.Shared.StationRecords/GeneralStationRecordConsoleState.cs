using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.StationRecords;

[Serializable]
[NetSerializable]
public sealed class GeneralStationRecordConsoleState : BoundUserInterfaceState
{
	public readonly uint? SelectedKey;

	public readonly GeneralStationRecord? Record;

	public readonly Dictionary<uint, string>? RecordListing;

	public readonly StationRecordsFilter? Filter;

	public readonly bool CanDeleteEntries;

	public GeneralStationRecordConsoleState(uint? key, GeneralStationRecord? record, Dictionary<uint, string>? recordListing, StationRecordsFilter? newFilter, bool canDeleteEntries)
	{
		SelectedKey = key;
		Record = record;
		RecordListing = recordListing;
		Filter = newFilter;
		CanDeleteEntries = canDeleteEntries;
	}

	public GeneralStationRecordConsoleState()
		: this(null, null, null, null, canDeleteEntries: false)
	{
	}

	public bool IsEmpty()
	{
		if (!SelectedKey.HasValue && Record == null)
		{
			return RecordListing == null;
		}
		return false;
	}
}
