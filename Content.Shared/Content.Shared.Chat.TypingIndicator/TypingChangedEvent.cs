using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chat.TypingIndicator;

[Serializable]
[NetSerializable]
public sealed class TypingChangedEvent : EntityEventArgs
{
	public readonly TypingIndicatorState State;

	public TypingChangedEvent(TypingIndicatorState state)
	{
		State = state;
	}
}
