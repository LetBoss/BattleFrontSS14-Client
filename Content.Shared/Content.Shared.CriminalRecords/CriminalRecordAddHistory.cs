using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CriminalRecords;

[Serializable]
[NetSerializable]
public sealed class CriminalRecordAddHistory : BoundUserInterfaceMessage
{
	public readonly string Line;

	public CriminalRecordAddHistory(string line)
	{
		Line = line;
	}
}
