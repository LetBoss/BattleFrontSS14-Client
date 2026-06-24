using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.StationRecords;

[Serializable]
[NetSerializable]
public sealed class DeleteStationRecord : BoundUserInterfaceMessage
{
	public readonly uint Id;

	public DeleteStationRecord(uint id)
	{
		Id = id;
	}
}
