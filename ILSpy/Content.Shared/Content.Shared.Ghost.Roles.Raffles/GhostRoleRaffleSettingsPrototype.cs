using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Ghost.Roles.Raffles;

[Prototype(null, 1)]
public sealed class GhostRoleRaffleSettingsPrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, true, false, null)]
	public GhostRoleRaffleSettings Settings { get; private set; } = new GhostRoleRaffleSettings();
}
