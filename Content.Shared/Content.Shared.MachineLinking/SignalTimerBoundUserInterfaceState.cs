using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MachineLinking;

[Serializable]
[NetSerializable]
public sealed class SignalTimerBoundUserInterfaceState : BoundUserInterfaceState
{
	public string CurrentText;

	public string CurrentDelayMinutes;

	public string CurrentDelaySeconds;

	public bool ShowText;

	public TimeSpan TriggerTime;

	public bool TimerStarted;

	public bool HasAccess;

	public SignalTimerBoundUserInterfaceState(string currentText, string currentDelayMinutes, string currentDelaySeconds, bool showText, TimeSpan triggerTime, bool timerStarted, bool hasAccess)
	{
		CurrentText = currentText;
		CurrentDelayMinutes = currentDelayMinutes;
		CurrentDelaySeconds = currentDelaySeconds;
		ShowText = showText;
		TriggerTime = triggerTime;
		TimerStarted = timerStarted;
		HasAccess = hasAccess;
	}
}
