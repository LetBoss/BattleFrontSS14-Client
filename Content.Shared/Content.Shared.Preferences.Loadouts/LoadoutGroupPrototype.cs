using System.Collections.Generic;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Preferences.Loadouts;

[Prototype(null, 1)]
public sealed class LoadoutGroupPrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public LocId Name;

	[DataField(null, false, 1, false, false, null)]
	public int MinLimit = 1;

	[DataField(null, false, 1, false, false, null)]
	public int MaxLimit = 1;

	[DataField(null, false, 1, false, false, null)]
	public bool Hidden;

	[DataField(null, false, 1, true, false, null)]
	public List<ProtoId<LoadoutPrototype>> Loadouts = new List<ProtoId<LoadoutPrototype>>();

	[IdDataField(1, null)]
	public string ID { get; private set; } = string.Empty;
}
