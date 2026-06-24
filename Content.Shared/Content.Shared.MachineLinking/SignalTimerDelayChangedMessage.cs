using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MachineLinking;

[Serializable]
[NetSerializable]
public sealed class SignalTimerDelayChangedMessage : BoundUserInterfaceMessage
{
	public TimeSpan Delay { get; }

	public SignalTimerDelayChangedMessage(TimeSpan delay)
	{
		Delay = delay;
	}
}
