using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Popups;

[Serializable]
[NetSerializable]
public sealed class PopupEntityEvent : PopupEvent
{
	public NetEntity Uid { get; }

	public PopupEntityEvent(string message, PopupType type, NetEntity uid)
		: base(message, type)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Uid = uid;
	}
}
