using Content.Shared.Radio;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Cargo.Prototypes;

[Prototype(null, 1)]
public sealed class CargoAccountPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public LocId Name;

	[DataField(null, false, 1, false, false, null)]
	public LocId Code;

	[DataField(null, false, 1, false, false, null)]
	public Color Color;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<RadioChannelPrototype> RadioChannel;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId AcquisitionSlip;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
