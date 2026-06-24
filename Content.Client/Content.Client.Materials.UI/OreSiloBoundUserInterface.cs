using System;
using Content.Shared.Materials.OreSilo;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Materials.UI;

public sealed class OreSiloBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private OreSiloMenu? _menu;

	public OreSiloBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<OreSiloMenu>((BoundUserInterface)(object)this);
		_menu.SetEntity(((BoundUserInterface)this).Owner);
		_menu.OnClientEntryPressed += delegate(NetEntity netEnt)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new ToggleOreSiloClientMessage(netEnt));
		};
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is OreSiloBuiState state2)
		{
			_menu?.Update(state2);
		}
	}
}
