using System.Collections.Generic;
using Content.Shared.Procedural;

namespace Content.Shared.Salvage.Magnet;

public record struct AsteroidOffering : ISalvageMagnetOffering
{
	public string Id;

	public DungeonConfig DungeonConfig;

	public Dictionary<string, int> MarkerLayers;
}
