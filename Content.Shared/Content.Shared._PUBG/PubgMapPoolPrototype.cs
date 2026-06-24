using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG;

[Prototype(null, 1)]
public sealed class PubgMapPoolPrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("maps", false, 1, true, false, null)]
	public List<string> Maps { get; private set; } = new List<string>();
}
