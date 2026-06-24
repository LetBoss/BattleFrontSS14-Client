using Robust.Shared.Noise;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Procedural.DungeonGenerators;

[DataRecord]
public record struct NoiseDunGenLayer
{
	[DataField(null, false, 1, false, false, null)]
	public float Threshold;

	[DataField(null, false, 1, true, false, null)]
	public string Tile;

	[DataField(null, false, 1, true, false, null)]
	public FastNoiseLite Noise;
}
