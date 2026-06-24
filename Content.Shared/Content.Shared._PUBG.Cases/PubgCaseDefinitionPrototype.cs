using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG.Cases;

[Prototype(null, 1)]
public sealed class PubgCaseDefinitionPrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; } = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public bool Enabled { get; private set; } = true;

	[DataField(null, false, 1, false, false, null)]
	public int DuplicateRecipeFallbackScrap { get; private set; }

	[DataField(null, false, 1, true, false, null)]
	public List<PubgCaseDropEntryData> Drops { get; private set; } = new List<PubgCaseDropEntryData>();
}
