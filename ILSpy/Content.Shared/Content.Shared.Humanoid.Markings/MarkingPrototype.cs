using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Humanoid.Markings;

[Prototype(null, 1)]
public sealed class MarkingPrototype : IPrototype
{
	[DataField("RMCFollowSkinColor", false, 1, false, false, null)]
	public bool RMCFollowSkinColor = true;

	[IdDataField(1, null)]
	public string ID { get; private set; } = "uwu";

	public string Name { get; private set; }

	[DataField("bodyPart", false, 1, true, false, null)]
	public HumanoidVisualLayers BodyPart { get; private set; }

	[DataField("markingCategory", false, 1, true, false, null)]
	public MarkingCategories MarkingCategory { get; private set; }

	[DataField("speciesRestriction", false, 1, false, false, null)]
	public List<string>? SpeciesRestrictions { get; private set; }

	[DataField("sexRestriction", false, 1, false, false, null)]
	public Sex? SexRestriction { get; private set; }

	[DataField("followSkinColor", false, 1, false, false, null)]
	public bool FollowSkinColor { get; private set; }

	[DataField("forcedColoring", false, 1, false, false, null)]
	public bool ForcedColoring { get; private set; }

	[DataField("coloring", false, 1, false, false, null)]
	public MarkingColors Coloring { get; private set; } = new MarkingColors();

	[DataField(null, false, 1, false, false, null)]
	public bool CanBeDisplaced { get; private set; } = true;

	[DataField("sprites", false, 1, true, false, null)]
	public List<SpriteSpecifier> Sprites { get; private set; }

	public Marking AsMarking()
	{
		return new Marking(ID, Sprites.Count);
	}
}
