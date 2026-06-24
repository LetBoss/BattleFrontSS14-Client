using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Communications;

[Prototype(null, 1)]
public sealed class NinjaHackingThreatPrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public LocId Announcement;

	[DataField(null, false, 1, true, false, null)]
	public EntProtoId Rule;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
