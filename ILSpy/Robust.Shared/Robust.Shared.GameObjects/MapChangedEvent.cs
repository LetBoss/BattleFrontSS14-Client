using System;
using Robust.Shared.Map;

namespace Robust.Shared.GameObjects;

[Obsolete("Use map creation or deletion events")]
public sealed class MapChangedEvent : EntityEventArgs
{
	public EntityUid Uid;

	public MapId Map { get; }

	public bool Created { get; }

	public bool Destroyed => !Created;

	public MapChangedEvent(EntityUid uid, MapId map, bool created)
	{
		Uid = uid;
		Map = map;
		Created = created;
	}
}
