using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Anomaly.Effects.Components;

[DataRecord]
public record struct EntitySpawnSettingsEntry()
{
	public List<EntProtoId> Spawns { get; set; } = new List<EntProtoId>();

	public AnomalySpawnSettings Settings { get; set; } = new AnomalySpawnSettings();
}
