using System;
using Robust.Shared.Serialization;

namespace Content.Shared.StationRecords;

[Serializable]
[NetSerializable]
public enum StationRecordFilterType : byte
{
	Name,
	Job,
	Species,
	Prints,
	DNA
}
