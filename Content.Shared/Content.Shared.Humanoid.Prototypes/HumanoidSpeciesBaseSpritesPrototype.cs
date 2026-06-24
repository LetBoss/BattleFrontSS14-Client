using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Humanoid.Prototypes;

[Prototype("speciesBaseSprites", 1)]
public sealed class HumanoidSpeciesBaseSpritesPrototype : IPrototype
{
	[DataField("sprites", false, 1, true, false, null)]
	public Dictionary<HumanoidVisualLayers, string> Sprites = new Dictionary<HumanoidVisualLayers, string>();

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
