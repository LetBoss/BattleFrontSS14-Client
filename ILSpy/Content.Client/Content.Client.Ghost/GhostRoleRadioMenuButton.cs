using Content.Client.UserInterface.Controls;
using Content.Shared.Ghost.Roles;
using Robust.Shared.Prototypes;

namespace Content.Client.Ghost;

public sealed class GhostRoleRadioMenuButton : RadialMenuTextureButtonWithSector
{
	public ProtoId<GhostRolePrototype> ProtoId { get; set; }
}
