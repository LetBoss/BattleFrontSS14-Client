using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Chat.TypingIndicator;

[Serializable]
[NetSerializable]
public enum TypingIndicatorState
{
	None,
	Idle,
	Typing
}
