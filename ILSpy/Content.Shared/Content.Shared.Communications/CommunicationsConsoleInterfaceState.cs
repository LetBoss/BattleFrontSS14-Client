using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Communications;

[Serializable]
[NetSerializable]
public sealed class CommunicationsConsoleInterfaceState : BoundUserInterfaceState
{
	public readonly bool CanAnnounce;

	public readonly bool CanBroadcast = true;

	public readonly bool CanCall;

	public readonly TimeSpan? ExpectedCountdownEnd;

	public readonly bool CountdownStarted;

	public List<string>? AlertLevels;

	public string CurrentAlert;

	public float CurrentAlertDelay;

	public CommunicationsConsoleInterfaceState(bool canAnnounce, bool canCall, List<string>? alertLevels, string currentAlert, float currentAlertDelay, TimeSpan? expectedCountdownEnd = null)
	{
		CanAnnounce = canAnnounce;
		CanCall = canCall;
		ExpectedCountdownEnd = expectedCountdownEnd;
		CountdownStarted = expectedCountdownEnd.HasValue;
		AlertLevels = alertLevels;
		CurrentAlert = currentAlert;
		CurrentAlertDelay = currentAlertDelay;
	}
}
