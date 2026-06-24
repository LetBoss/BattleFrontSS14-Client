using Content.Shared.Maps;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Anomaly.Effects.Components;

[DataRecord]
public record struct TileSpawnSettingsEntry()
{
	public ProtoId<ContentTileDefinition> Floor { get; set; } = default(ProtoId<ContentTileDefinition>);

	public AnomalySpawnSettings Settings { get; set; } = new AnomalySpawnSettings();
}
