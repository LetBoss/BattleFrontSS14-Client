using System;

namespace Robust.Shared.GameObjects;

public sealed class BoundUIClosedEvent : BaseLocalBoundUserInterfaceEvent
{
	public BoundUIClosedEvent(Enum uiKey, EntityUid uid, EntityUid actor)
	{
		UiKey = uiKey;
		Entity = uid;
		Actor = actor;
	}
}
