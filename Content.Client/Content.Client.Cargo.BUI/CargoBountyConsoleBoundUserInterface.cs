using System;
using Content.Client.Cargo.UI;
using Content.Shared.Cargo.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Cargo.BUI;

public sealed class CargoBountyConsoleBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private CargoBountyMenu? _menu;

	public CargoBountyConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<CargoBountyMenu>((BoundUserInterface)(object)this);
		CargoBountyMenu? menu = _menu;
		menu.OnLabelButtonPressed = (Action<string>)Delegate.Combine(menu.OnLabelButtonPressed, (Action<string>)delegate(string id)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new BountyPrintLabelMessage(id));
		});
		CargoBountyMenu? menu2 = _menu;
		menu2.OnSkipButtonPressed = (Action<string>)Delegate.Combine(menu2.OnSkipButtonPressed, (Action<string>)delegate(string id)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new BountySkipMessage(id));
		});
	}

	protected override void UpdateState(BoundUserInterfaceState message)
	{
		((BoundUserInterface)this).UpdateState(message);
		if (message is CargoBountyConsoleState cargoBountyConsoleState)
		{
			_menu?.UpdateEntries(cargoBountyConsoleState.Bounties, cargoBountyConsoleState.History, cargoBountyConsoleState.UntilNextSkip);
		}
	}
}
