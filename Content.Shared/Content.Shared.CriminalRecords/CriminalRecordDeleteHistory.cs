using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CriminalRecords;

[Serializable]
[NetSerializable]
public sealed class CriminalRecordDeleteHistory : BoundUserInterfaceMessage
{
	public readonly uint Index;

	public CriminalRecordDeleteHistory(uint index)
	{
		Index = index;
	}
}
