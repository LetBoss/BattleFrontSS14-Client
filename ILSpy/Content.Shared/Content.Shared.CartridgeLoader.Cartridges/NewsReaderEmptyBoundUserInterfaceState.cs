using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable]
[NetSerializable]
public sealed class NewsReaderEmptyBoundUserInterfaceState : BoundUserInterfaceState
{
	public bool NotificationOn;

	public NewsReaderEmptyBoundUserInterfaceState(bool notificationOn)
	{
		NotificationOn = notificationOn;
	}
}
