using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Ranged.Events;

[Serializable]
[NetSerializable]
public sealed class RequestStopShootEvent : EntityEventArgs
{
	public NetEntity Gun;
}
