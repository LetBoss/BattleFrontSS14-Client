using System;
using Content.Shared.Chat.TypingIndicator;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Holopad;

[Serializable]
[NetSerializable]
public sealed class HolopadUserTypingChangedEvent : EntityEventArgs
{
	public readonly NetEntity User;

	public readonly TypingIndicatorState State;

	public HolopadUserTypingChangedEvent(NetEntity user, TypingIndicatorState state)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		User = user;
		State = state;
	}
}
