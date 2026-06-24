using System;
using Robust.Shared.Enums;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.StationRecords;

[Serializable]
[NetSerializable]
public sealed record GeneralStationRecord
{
	[DataField(null, false, 1, false, false, null)]
	public string Name = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public int Age;

	[DataField(null, false, 1, false, false, null)]
	public string JobTitle = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public string JobIcon = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public string JobPrototype = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public string? Squad;

	[DataField(null, false, 1, false, false, null)]
	public string Species = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public Gender Gender = (Gender)1;

	[DataField(null, false, 1, false, false, null)]
	public int DisplayPriority;

	[DataField(null, false, 1, false, false, null)]
	public string? Fingerprint;

	[DataField(null, false, 1, false, false, null)]
	public string? DNA;
}
