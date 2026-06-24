using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Station;

[Serializable]
[NetSerializable]
public sealed class StationsUpdatedEvent : EntityEventArgs
{
	public readonly List<(string Name, NetEntity Entity)> Stations;

	public StationsUpdatedEvent(List<(string Name, NetEntity Entity)> stations)
	{
		Stations = stations;
	}
}
