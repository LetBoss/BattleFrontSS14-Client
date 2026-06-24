using System;
using Content.Shared.Security;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CriminalRecords;

[Serializable]
[NetSerializable]
public sealed class CriminalRecordChangeStatus : BoundUserInterfaceMessage
{
	public readonly SecurityStatus Status;

	public readonly string? Reason;

	public CriminalRecordChangeStatus(SecurityStatus status, string? reason)
	{
		Status = status;
		Reason = reason;
	}
}
