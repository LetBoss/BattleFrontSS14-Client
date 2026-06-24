using System.Collections.Generic;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Research.Prototypes;

[Prototype(null, 1)]
public sealed class TechDisciplinePrototype : IPrototype
{
	[DataField("name", false, 1, true, false, null)]
	public string Name = string.Empty;

	[DataField("color", false, 1, true, false, null)]
	public Color Color;

	[DataField("icon", false, 1, false, false, null)]
	public SpriteSpecifier Icon;

	[DataField("tierPrerequisites", false, 1, true, false, null)]
	public Dictionary<int, float> TierPrerequisites = new Dictionary<int, float>();

	[DataField("lockoutTier", false, 1, false, false, null)]
	public int LockoutTier = 3;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
