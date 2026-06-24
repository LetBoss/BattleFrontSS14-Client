using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Popups;

[Serializable]
[NetSerializable]
public sealed class PopupCursorEvent : PopupEvent
{
	public PopupCursorEvent(string message, PopupType type)
		: base(message, type)
	{
	}
}
