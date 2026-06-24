using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.BarSign;

[Prototype(null, 1)]
public sealed class BarSignPrototype : IPrototype
{
	[IdDataField(1, null)]
	[ViewVariables]
	public string ID { get; private set; }

	[DataField(null, false, 1, true, false, null)]
	public SpriteSpecifier Icon { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public LocId Name { get; private set; } = LocId.op_Implicit("barsign-component-name");

	[DataField(null, false, 1, false, false, null)]
	public LocId Description { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public bool Hidden { get; private set; }
}
