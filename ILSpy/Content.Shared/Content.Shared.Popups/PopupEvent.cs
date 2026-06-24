using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Popups;

[Serializable]
[NetSerializable]
public abstract class PopupEvent : EntityEventArgs
{
	public string Message { get; }

	public PopupType Type { get; }

	protected PopupEvent(string message, PopupType type)
	{
		Message = message;
		Type = type;
	}
}
