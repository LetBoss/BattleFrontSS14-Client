using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Chemistry.Reaction;

[Prototype(null, 1)]
public sealed class MixingCategoryPrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public LocId VerbText;

	[DataField(null, false, 1, true, false, null)]
	public SpriteSpecifier Icon;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
