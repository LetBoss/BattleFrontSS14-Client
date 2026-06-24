using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Events;

[Serializable]
[NetSerializable]
public sealed class PlayerInfoChangedEvent : EntityEventArgs
{
	public PlayerInfo? PlayerInfo;
}
