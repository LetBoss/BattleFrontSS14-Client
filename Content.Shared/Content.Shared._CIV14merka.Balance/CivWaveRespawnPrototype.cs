using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._CIV14merka.Balance;

[Prototype(null, 1)]
public sealed class CivWaveRespawnPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public bool Enabled = true;

	[DataField(null, false, 1, false, false, null)]
	public int BaseIntervalSeconds = 300;

	[DataField(null, false, 1, false, false, null)]
	public int MinIntervalSeconds = 180;

	[DataField(null, false, 1, false, false, null)]
	public int MaxIntervalSeconds = 420;

	[DataField(null, false, 1, false, false, null)]
	public int MinGhostsToTrigger = 2;

	[DataField(null, false, 1, false, false, null)]
	public float ScoreRatioForMax = 2f;

	[DataField(null, false, 1, false, false, null)]
	public int ConfirmWindowBaseSeconds = 20;

	[DataField(null, false, 1, false, false, null)]
	public int ConfirmWindowMinSeconds = 10;

	[DataField(null, false, 1, false, false, null)]
	public int ConfirmWindowMaxSeconds = 30;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
