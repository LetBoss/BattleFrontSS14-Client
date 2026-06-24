using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._CIV14merka.Commander;

[Prototype(null, 1)]
public sealed class CivArtilleryConfigPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public int ExplosionCount = 3;

	[DataField(null, false, 1, false, false, null)]
	public float ScatterMin = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float ScatterMax = 3f;

	[DataField(null, false, 1, false, false, null)]
	public float DelaySeconds = 2f;

	[DataField(null, false, 1, false, false, null)]
	public int CooldownSeconds = 30;

	[DataField(null, false, 1, false, false, null)]
	public int BypassCost = 200;

	[DataField(null, false, 1, false, false, null)]
	public float ExplosionIntensity = 175f;

	[DataField(null, false, 1, false, false, null)]
	public float ExplosionSlope = 5f;

	[DataField(null, false, 1, false, false, null)]
	public float ExplosionMaxTileIntensity = 25f;

	[DataField(null, false, 1, false, false, null)]
	public string ExplosionType = "RMC";

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? WarningSound;

	[DataField(null, false, 1, false, false, null)]
	public string WarningPopup = "АРТУДАР!";

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
