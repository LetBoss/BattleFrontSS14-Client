using System;
using Robust.Shared.Serialization;

namespace Content.Shared.StationRecords;

[Serializable]
[NetSerializable]
public sealed class StationRecordsFilter
{
	public StationRecordFilterType Type;

	public string Value = "";

	public StationRecordsFilter(StationRecordFilterType filterType, string newValue = "")
	{
		Type = filterType;
		Value = newValue;
	}
}
