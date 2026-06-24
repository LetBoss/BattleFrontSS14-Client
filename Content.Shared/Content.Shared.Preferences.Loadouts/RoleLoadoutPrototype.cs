using System.Collections.Generic;
using Content.Shared.Dataset;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Preferences.Loadouts;

[Prototype(null, 1)]
public sealed class RoleLoadoutPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public bool CanCustomizeName;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<LocalizedDatasetPrototype>? NameDataset;

	[DataField(null, false, 1, false, false, null)]
	public List<ProtoId<LoadoutGroupPrototype>> Groups = new List<ProtoId<LoadoutGroupPrototype>>();

	[DataField(null, false, 1, false, false, null)]
	public int? Points;

	[IdDataField(1, null)]
	public string ID { get; private set; } = string.Empty;
}
