using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.StationRecords;

[Serializable]
[NetSerializable]
public sealed class StationRecordKeyStorageComponentState : ComponentState
{
	public (NetEntity, uint)? Key;

	public StationRecordKeyStorageComponentState((NetEntity, uint)? key)
	{
		Key = key;
	}
}
