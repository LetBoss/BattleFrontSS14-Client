using Content.Shared.Whitelist;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Xenoarchaeology.Artifact.Prototypes;

[Prototype(null, 1)]
public sealed class XenoArchTriggerPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public LocId Tip;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Whitelist;

	[DataField(null, false, 1, false, false, null)]
	public ComponentRegistry Components = new ComponentRegistry();

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
