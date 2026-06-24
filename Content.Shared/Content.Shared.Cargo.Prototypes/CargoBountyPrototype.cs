using System.Collections.Generic;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Cargo.Prototypes;

[Prototype(null, 1)]
public sealed class CargoBountyPrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public int Reward;

	[DataField(null, false, 1, false, false, null)]
	public LocId Description = LocId.op_Implicit(string.Empty);

	[DataField(null, false, 1, true, false, null)]
	public List<CargoBountyItemEntry> Entries = new List<CargoBountyItemEntry>();

	[DataField(null, false, 1, false, false, null)]
	public string IdPrefix = "NT";

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<CargoBountyGroupPrototype> Group = ProtoId<CargoBountyGroupPrototype>.op_Implicit("StationBounty");

	[DataField(null, false, 1, false, false, null)]
	public SpriteSpecifier? Sprite;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
