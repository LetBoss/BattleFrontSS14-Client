using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Players.PlayTimeTracking;

[Prototype(null, 1)]
public sealed class PlayTimeTrackerPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public bool IsHumanoid;

	[DataField(null, false, 1, false, false, null)]
	public bool IsXeno;

	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public LocId? Name { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public bool ShowInStatsMenu { get; private set; } = true;
}
