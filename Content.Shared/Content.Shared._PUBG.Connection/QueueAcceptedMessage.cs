using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Connection;

[Serializable]
[NetSerializable]
public sealed class QueueAcceptedMessage : EntityEventArgs
{
	public bool OpenModeMenu { get; }

	public QueueAcceptedMessage(bool openModeMenu = false)
	{
		OpenModeMenu = openModeMenu;
	}
}
