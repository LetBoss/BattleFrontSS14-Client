using System;
using Content.Client.Power.APC.UI;
using Content.Shared.Access.Systems;
using Content.Shared.APC;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Power.APC;

public sealed class ApcBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private ApcMenu? _menu;

	public ApcBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<ApcMenu>((BoundUserInterface)(object)this);
		_menu.SetEntity(((BoundUserInterface)this).Owner);
		_menu.OnBreaker += BreakerPressed;
		bool accessEnabled = false;
		if (base.PlayerManager.LocalEntity.HasValue)
		{
			accessEnabled = base.EntMan.System<AccessReaderSystem>().IsAllowed(base.PlayerManager.LocalEntity.Value, ((BoundUserInterface)this).Owner);
		}
		_menu?.SetAccessEnabled(accessEnabled);
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		ApcBoundInterfaceState state2 = (ApcBoundInterfaceState)(object)state;
		_menu?.UpdateState((BoundUserInterfaceState)(object)state2);
	}

	public void BreakerPressed()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ApcToggleMainBreakerMessage());
	}
}
