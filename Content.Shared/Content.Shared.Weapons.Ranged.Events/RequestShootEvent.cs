using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared.Weapons.Ranged.Events;

[Serializable]
[NetSerializable]
public sealed class RequestShootEvent : EntityEventArgs
{
	public NetEntity Gun;

	public NetCoordinates Coordinates;

	public NetEntity? Target;

	public List<int>? Shot;

	public GameTick LastRealTick;
}
