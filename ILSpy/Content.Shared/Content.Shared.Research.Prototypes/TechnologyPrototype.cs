using System.Collections.Generic;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Research.Prototypes;

[Prototype(null, 1)]
public sealed class TechnologyPrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public LocId Name = LocId.op_Implicit(string.Empty);

	[DataField(null, false, 1, true, false, null)]
	public SpriteSpecifier Icon;

	[DataField(null, false, 1, true, false, null)]
	public ProtoId<TechDisciplinePrototype> Discipline;

	[DataField(null, false, 1, true, false, null)]
	public int Tier;

	[DataField(null, false, 1, false, false, null)]
	public bool Hidden;

	[DataField(null, false, 1, false, false, null)]
	public int Cost = 10000;

	[DataField(null, false, 1, false, false, null)]
	public List<ProtoId<TechnologyPrototype>> TechnologyPrerequisites = new List<ProtoId<TechnologyPrototype>>();

	[DataField(null, false, 1, false, false, null)]
	public List<ProtoId<LatheRecipePrototype>> RecipeUnlocks = new List<ProtoId<LatheRecipePrototype>>();

	[DataField(null, false, 1, false, false, null)]
	public IReadOnlyList<GenericUnlock> GenericUnlocks = new List<GenericUnlock>();

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
