using System.Collections.Generic;
using Content.Shared.Dataset;
using Content.Shared.Humanoid.Markings;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Humanoid.Prototypes;

[Prototype(null, 1)]
public sealed class SpeciesPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public int MinAge = 18;

	[DataField(null, false, 1, false, false, null)]
	public int YoungAge = 30;

	[DataField(null, false, 1, false, false, null)]
	public int OldAge = 60;

	[DataField(null, false, 1, false, false, null)]
	public int MaxAge = 120;

	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, true, false, null)]
	public string Name { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public string Descriptor { get; private set; } = "humanoid";

	[DataField(null, false, 1, true, false, null)]
	public bool RoundStart { get; private set; }

	[DataField("sprites", false, 1, false, false, null)]
	public ProtoId<HumanoidSpeciesBaseSpritesPrototype> SpriteSet { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public Color DefaultSkinTone { get; private set; } = Color.White;

	[DataField(null, false, 1, false, false, null)]
	public int DefaultHumanSkinTone { get; private set; } = 20;

	[DataField("markingLimits", false, 1, false, false, null)]
	public ProtoId<MarkingPointsPrototype> MarkingPoints { get; private set; }

	[DataField(null, false, 1, true, false, null)]
	public EntProtoId Prototype { get; private set; }

	[DataField(null, false, 1, true, false, null)]
	public EntProtoId DollPrototype { get; private set; }

	[DataField(null, false, 1, true, false, null)]
	public HumanoidSkinColor SkinColoration { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<LocalizedDatasetPrototype> MaleFirstNames { get; private set; } = ProtoId<LocalizedDatasetPrototype>.op_Implicit("NamesFirstMale");

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<LocalizedDatasetPrototype> FemaleFirstNames { get; private set; } = ProtoId<LocalizedDatasetPrototype>.op_Implicit("NamesFirstFemale");

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<LocalizedDatasetPrototype> LastNames { get; private set; } = ProtoId<LocalizedDatasetPrototype>.op_Implicit("NamesLast");

	[DataField(null, false, 1, false, false, null)]
	public SpeciesNaming Naming { get; private set; } = SpeciesNaming.FirstLast;

	[DataField(null, false, 1, false, false, null)]
	public List<Sex> Sexes { get; private set; } = new List<Sex>
	{
		Sex.Male,
		Sex.Female
	};
}
