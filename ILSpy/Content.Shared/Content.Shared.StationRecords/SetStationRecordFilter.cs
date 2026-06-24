using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.StationRecords;

[Serializable]
[NetSerializable]
public sealed class SetStationRecordFilter : BoundUserInterfaceMessage
{
	public readonly string Value;

	public readonly StationRecordFilterType Type;

	public SetStationRecordFilter(StationRecordFilterType filterType, string filterValue)
	{
		Type = filterType;
		Value = filterValue;
	}
}
