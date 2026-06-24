using System;
using Content.Shared.Security;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CriminalRecords;

[Serializable]
[NetSerializable]
public sealed class CriminalRecordSetStatusFilter : BoundUserInterfaceMessage
{
	public readonly SecurityStatus FilterStatus;

	public CriminalRecordSetStatusFilter(SecurityStatus newFilterStatus)
	{
		FilterStatus = newFilterStatus;
	}
}
