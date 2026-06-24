using System.Collections.Generic;
using Content.Shared.Parallax.Biomes.Layers;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Parallax.Biomes;

[Prototype(null, 1)]
public sealed class BiomeTemplatePrototype : IPrototype
{
	[DataField("layers", false, 1, false, false, null)]
	public List<IBiomeLayer> Layers = new List<IBiomeLayer>();

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
