using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared._PUBG.MapGen;

[Prototype(null, 1)]
public sealed class PubgBuildingPrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, true, false, null)]
	public string Path { get; private set; }

	public ResPath FullPath => new ResPath("/Maps/_PUBG/Buildings/" + Path);

	[DataField(null, false, 1, false, false, null)]
	public bool Enabled { get; private set; } = true;

	[DataField(null, false, 1, false, false, null)]
	public bool CanRotate { get; private set; } = true;
}
