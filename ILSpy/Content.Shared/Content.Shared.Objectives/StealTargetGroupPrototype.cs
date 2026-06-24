using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Objectives;

[Prototype(null, 1)]
public sealed class StealTargetGroupPrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public LocId Name { get; private set; } = LocId.op_Implicit(string.Empty);

	[DataField(null, false, 1, false, false, null)]
	public SpriteSpecifier Sprite { get; private set; } = SpriteSpecifier.Invalid;
}
