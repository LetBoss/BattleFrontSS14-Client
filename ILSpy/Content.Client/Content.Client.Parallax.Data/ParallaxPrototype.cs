using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Parallax.Data;

[Prototype(null, 1)]
public sealed class ParallaxPrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("layers", false, 1, false, false, null)]
	public List<ParallaxLayerConfig> Layers { get; private set; } = new List<ParallaxLayerConfig>();

	[DataField("layersLQ", false, 1, false, false, null)]
	public List<ParallaxLayerConfig> LayersLQ { get; private set; } = new List<ParallaxLayerConfig>();

	[DataField("layersLQUseHQ", false, 1, false, false, null)]
	public bool LayersLQUseHQ { get; private set; } = true;
}
