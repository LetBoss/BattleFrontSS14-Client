using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.StationRecords;

[Serializable]
[NetSerializable]
public sealed class SelectStationRecord : BoundUserInterfaceMessage
{
	public readonly uint? SelectedKey;

	public SelectStationRecord(uint? selectedKey)
	{
		SelectedKey = selectedKey;
	}
}
