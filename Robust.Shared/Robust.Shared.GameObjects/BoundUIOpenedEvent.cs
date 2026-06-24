using System;

namespace Robust.Shared.GameObjects;

public sealed class BoundUIOpenedEvent : BaseLocalBoundUserInterfaceEvent
{
	public BoundUIOpenedEvent(Enum uiKey, EntityUid uid, EntityUid actor)
	{
		UiKey = uiKey;
		Entity = uid;
		Actor = actor;
	}
}
