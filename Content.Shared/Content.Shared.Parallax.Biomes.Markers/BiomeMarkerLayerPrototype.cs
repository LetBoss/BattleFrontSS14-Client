using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Parallax.Biomes.Markers;

[Prototype(null, 1)]
public sealed class BiomeMarkerLayerPrototype : IBiomeMarkerLayer, IPrototype
{
	[DataField("radius", false, 1, false, false, null)]
	public float Radius = 32f;

	[DataField("maxCount", false, 1, false, false, null)]
	public int MaxCount = int.MaxValue;

	[DataField(null, false, 1, false, false, null)]
	public int MinGroupSize = 1;

	[DataField(null, false, 1, false, false, null)]
	public int MaxGroupSize = 1;

	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<EntProtoId, EntProtoId> EntityMask { get; private set; } = new Dictionary<EntProtoId, EntProtoId>();

	[DataField(null, false, 1, false, false, null)]
	public string? Prototype { get; private set; }

	[DataField("size", false, 1, false, false, null)]
	public int Size { get; private set; } = 128;
}
