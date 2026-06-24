using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Anomaly;

[DataRecord]
public record struct AnomalySpawnSettings()
{
	public bool CanSpawnOnEntities { get; set; } = false;

	public int MinAmount { get; set; } = 0;

	public int MaxAmount { get; set; } = 1;

	public float MinRange { get; set; } = 0f;

	public float MaxRange { get; set; } = 1f;

	public bool SpawnOnPulse { get; set; } = false;

	public bool SpawnOnSuperCritical { get; set; } = false;

	public bool SpawnOnShutdown { get; set; } = false;

	public bool SpawnOnStabilityChanged { get; set; } = false;

	public bool SpawnOnSeverityChanged { get; set; } = false;
}
