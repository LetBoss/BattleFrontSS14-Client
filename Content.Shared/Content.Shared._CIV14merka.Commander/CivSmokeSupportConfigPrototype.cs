using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._CIV14merka.Commander;

[Prototype(null, 1)]
public sealed class CivSmokeSupportConfigPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public int PuffCount = 8;

	[DataField(null, false, 1, false, false, null)]
	public float PuffSpacing = 2f;

	[DataField(null, false, 1, false, false, null)]
	public int PuffRange = 3;

	[DataField(null, false, 1, false, false, null)]
	public float SmokeDuration = 60f;

	[DataField(null, false, 1, false, false, null)]
	public float DelaySeconds = 3f;

	[DataField(null, false, 1, false, false, null)]
	public int CooldownSeconds = 60;

	[DataField(null, false, 1, false, false, null)]
	public int BypassCost;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId SmokeEntity = EntProtoId.op_Implicit("CivCommanderSmokePuff");

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? WarningSound;

	[DataField(null, false, 1, false, false, null)]
	public string WarningPopup = "ДЫМ!";

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
