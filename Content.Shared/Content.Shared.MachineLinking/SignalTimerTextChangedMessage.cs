using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MachineLinking;

[Serializable]
[NetSerializable]
public sealed class SignalTimerTextChangedMessage : BoundUserInterfaceMessage
{
	public string Text { get; }

	public SignalTimerTextChangedMessage(string text)
	{
		Text = text;
	}
}
