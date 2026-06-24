using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared._PUBG.Sponsor;

[Prototype(null, 1)]
public sealed class PubgSponsorSandboxMapPrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public ResPath MapPath;

	[DataField(null, false, 1, false, false, null)]
	public List<string> Ckeys = new List<string>();

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<string, List<string>> Permissions = new Dictionary<string, List<string>>();

	[DataField(null, false, 1, false, false, null)]
	public List<string> DisallowedEntityIds = new List<string>();

	[DataField(null, false, 1, false, false, null)]
	public bool BlockEraseMinds;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
