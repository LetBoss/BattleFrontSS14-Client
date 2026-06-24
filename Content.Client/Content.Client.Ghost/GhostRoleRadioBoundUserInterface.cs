using System;
using Content.Shared.Ghost.Roles;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Ghost;

public sealed class GhostRoleRadioBoundUserInterface : BoundUserInterface
{
	private GhostRoleRadioMenu? _ghostRoleRadioMenu;

	public GhostRoleRadioBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<GhostRoleRadioBoundUserInterface>(this);
	}

	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_ghostRoleRadioMenu = BoundUserInterfaceExt.CreateWindow<GhostRoleRadioMenu>((BoundUserInterface)(object)this);
		_ghostRoleRadioMenu.SetEntity(((BoundUserInterface)this).Owner);
		_ghostRoleRadioMenu.SendGhostRoleRadioMessageAction += SendGhostRoleRadioMessage;
	}

	private void SendGhostRoleRadioMessage(ProtoId<GhostRolePrototype> protoId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new GhostRoleRadioMessage(protoId));
	}
}
