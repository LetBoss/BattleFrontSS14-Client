using System.Collections.Generic;
using Content.Shared.Preferences;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Humanoid.Prototypes;

[Prototype(null, 1)]
public sealed class HumanoidProfilePrototype : IPrototype
{
	[DataField("customBaseLayers", false, 1, false, false, null)]
	public Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> CustomBaseLayers = new Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>();

	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("profile", false, 1, false, false, null)]
	public HumanoidCharacterProfile Profile { get; private set; } = new HumanoidCharacterProfile();
}
