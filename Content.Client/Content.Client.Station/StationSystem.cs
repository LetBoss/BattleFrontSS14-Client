using System;
using System.Collections.Generic;
using Content.Shared.Station;
using Robust.Shared.GameObjects;

namespace Content.Client.Station;

public sealed class StationSystem : SharedStationSystem
{
	private readonly List<(string Name, NetEntity Entity)> _stations = new List<(string, NetEntity)>();

	public IReadOnlyList<(string Name, NetEntity Entity)> Stations => _stations;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<StationsUpdatedEvent>((EntityEventHandler<StationsUpdatedEvent>)StationsUpdated, (Type[])null, (Type[])null);
	}

	private void StationsUpdated(StationsUpdatedEvent ev)
	{
		_stations.Clear();
		_stations.AddRange(ev.Stations);
	}
}
