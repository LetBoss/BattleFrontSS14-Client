using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG.MiniGames;

[Prototype(null, 1)]
public sealed class PubgMiniGameBlacklistPrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; } = "MiniGameDefault";

	[DataField(null, false, 1, false, false, null)]
	public List<string> DisallowedEntityIds { get; private set; } = new List<string>();
}
