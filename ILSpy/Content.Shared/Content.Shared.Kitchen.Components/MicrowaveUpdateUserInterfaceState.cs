using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen.Components;

[Serializable]
[NetSerializable]
public sealed class MicrowaveUpdateUserInterfaceState : BoundUserInterfaceState
{
	public NetEntity[] ContainedSolids;

	public bool IsMicrowaveBusy;

	public int ActiveButtonIndex;

	public uint CurrentCookTime;

	public TimeSpan CurrentCookTimeEnd;

	public MicrowaveUpdateUserInterfaceState(NetEntity[] containedSolids, bool isMicrowaveBusy, int activeButtonIndex, uint currentCookTime, TimeSpan currentCookTimeEnd)
	{
		ContainedSolids = containedSolids;
		IsMicrowaveBusy = isMicrowaveBusy;
		ActiveButtonIndex = activeButtonIndex;
		CurrentCookTime = currentCookTime;
		CurrentCookTimeEnd = currentCookTimeEnd;
	}
}
