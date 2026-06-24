using System;
using Content.Client.Cargo.UI;
using Content.Shared.Cargo.BUI;
using Content.Shared.Cargo.Events;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Cargo.BUI;

public sealed class CargoPalletConsoleBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private CargoPalletMenu? _menu;

	public CargoPalletConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<CargoPalletMenu>((BoundUserInterface)(object)this);
		CargoPalletMenu? menu = _menu;
		menu.AppraiseRequested = (Action)Delegate.Combine(menu.AppraiseRequested, new Action(OnAppraisal));
		CargoPalletMenu? menu2 = _menu;
		menu2.SellRequested = (Action)Delegate.Combine(menu2.SellRequested, new Action(OnSell));
	}

	private void OnAppraisal()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CargoPalletAppraiseMessage());
	}

	private void OnSell()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CargoPalletSellMessage());
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is CargoPalletConsoleInterfaceState cargoPalletConsoleInterfaceState)
		{
			_menu?.SetEnabled(cargoPalletConsoleInterfaceState.Enabled);
			_menu?.SetAppraisal(cargoPalletConsoleInterfaceState.Appraisal);
			_menu?.SetCount(cargoPalletConsoleInterfaceState.Count);
		}
	}
}
