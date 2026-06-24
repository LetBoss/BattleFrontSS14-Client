using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Anomaly.Prototypes;

[Prototype(null, 1)]
public sealed class AnomalyBehaviorPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public string Description = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public float EarnPointModifier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float PulseFrequencyModifier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float PulsePowerModifier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float ParticleSensivity = 1f;

	[DataField(null, false, 1, false, true, null)]
	public ComponentRegistry Components = new ComponentRegistry();

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
