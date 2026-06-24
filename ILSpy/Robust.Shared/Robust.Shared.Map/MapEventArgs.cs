using System;

namespace Robust.Shared.Map;

public sealed class MapEventArgs : EventArgs
{
	public MapId Map { get; }

	public MapEventArgs(MapId map)
	{
		Map = map;
	}
}
