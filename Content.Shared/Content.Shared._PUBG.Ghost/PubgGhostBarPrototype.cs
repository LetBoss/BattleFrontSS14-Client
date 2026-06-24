using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared._PUBG.Ghost;

[Prototype(null, 1)]
public sealed class PubgGhostBarPrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("mapPath", false, 1, true, false, null)]
	public ResPath MapPath { get; private set; }
}
