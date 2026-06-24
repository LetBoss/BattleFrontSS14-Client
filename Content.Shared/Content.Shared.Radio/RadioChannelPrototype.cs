using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Radio;

[Prototype(null, 1)]
public sealed class RadioChannelPrototype : IPrototype
{
	[DataField("longRange", false, 1, false, false, null)]
	[ViewVariables]
	public bool LongRange;

	[DataField(null, false, 1, false, false, null)]
	public bool Tower;

	[DataField(null, false, 1, false, false, null)]
	public bool Planet = true;

	[DataField(null, false, 1, false, false, null)]
	public Color? ColorblindColor;

	[DataField("name", false, 1, false, false, null)]
	public LocId Name { get; private set; } = LocId.op_Implicit(string.Empty);

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string LocalizedName => Loc.GetString(LocId.op_Implicit(Name));

	[DataField("keycode", false, 1, false, false, null)]
	public char KeyCode { get; private set; }

	[DataField("frequency", false, 1, false, false, null)]
	public int Frequency { get; private set; }

	[DataField("color", false, 1, false, false, null)]
	public Color Color { get; private set; } = Color.Lime;

	[IdDataField(1, null)]
	[ViewVariables]
	public string ID { get; private set; }
}
