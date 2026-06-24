using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared._PUBG;

[Prototype(null, 1)]
public sealed class PubgLobbyMapPrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public ResPath MapPath;

	[DataField(null, false, 1, false, false, null)]
	public int MinPlayers = 1;

	[DataField(null, false, 1, false, false, null)]
	public string Name = string.Empty;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
