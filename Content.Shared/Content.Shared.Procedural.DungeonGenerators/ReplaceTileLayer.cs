using Content.Shared.Maps;
using Robust.Shared.Noise;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Procedural.DungeonGenerators;

[DataRecord]
public record struct ReplaceTileLayer
{
	public ProtoId<ContentTileDefinition> Tile;

	public float Threshold;

	public FastNoiseLite Noise;
}
