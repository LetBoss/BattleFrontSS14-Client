using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.NukeOps;

[Serializable]
[NetSerializable]
public sealed class WarDeclaratorBoundUserInterfaceState : BoundUserInterfaceState
{
	public WarConditionStatus? Status;

	public TimeSpan ShuttleDisabledTime;

	public TimeSpan EndTime;

	public WarDeclaratorBoundUserInterfaceState(WarConditionStatus? status, TimeSpan endTime, TimeSpan shuttleDisabledTime)
	{
		Status = status;
		EndTime = endTime;
		ShuttleDisabledTime = shuttleDisabledTime;
	}
}
