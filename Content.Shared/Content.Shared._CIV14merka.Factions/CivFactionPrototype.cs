using System.Collections.Generic;
using Content.Shared._CIV14merka.PurchaseRequest;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.Roles;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared._CIV14merka.Factions;

[Prototype(null, 1)]
public sealed class CivFactionPrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public string Name = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public string ShortTag = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public Color Color = Color.White;

	[DataField(null, false, 1, false, false, null)]
	public SpriteSpecifier? Icon;

	[DataField(null, false, 1, true, false, null)]
	public Dictionary<CivTdmClass, ProtoId<StartingGearPrototype>> Loadouts = new Dictionary<CivTdmClass, ProtoId<StartingGearPrototype>>();

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<StartingGearPrototype>? BotGear;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<CivPurchaseCatalogPrototype>? PurchaseCatalog;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<CivSupplyContentPrototype>? SupplyContent;

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<string, EntProtoId> SpawnEntities = new Dictionary<string, EntProtoId>();

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
