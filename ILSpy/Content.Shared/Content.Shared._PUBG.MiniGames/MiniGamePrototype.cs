using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG.MiniGames;

[Prototype("pubgMiniGame", 1)]
public sealed class MiniGamePrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; } = string.Empty;

	[DataField(null, false, 1, true, false, null)]
	public string Name { get; private set; } = string.Empty;

	[DataField(null, false, 1, true, false, null)]
	public string Description { get; private set; } = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public string MapName { get; private set; } = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public string MapPath { get; private set; } = string.Empty;

	[DataField(null, false, 1, true, false, null)]
	public int MaxPlayers { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public int MinPlayers { get; private set; } = 2;

	[DataField(null, false, 1, false, false, null)]
	public int DefaultRounds { get; private set; } = 1;

	[DataField(null, false, 1, false, false, null)]
	public int MaxRounds { get; private set; } = 10;

	[DataField(null, false, 1, false, false, null)]
	public int TeamCount { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public List<MiniGameMapDefinition> Maps { get; private set; } = new List<MiniGameMapDefinition>();
}
