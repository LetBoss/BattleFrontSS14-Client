using System;
using Content.Shared.Wires;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Wires.UI;

public sealed class WiresBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private WiresMenu? _menu;

	public WiresBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<WiresMenu>((BoundUserInterface)(object)this);
		_menu.OnAction += PerformAction;
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		_menu?.Populate((WiresBoundUserInterfaceState)(object)state);
	}

	public void PerformAction(int id, WiresAction action)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new WiresActionMessage(id, action));
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			WiresMenu? menu = _menu;
			if (menu != null)
			{
				((Control)menu).Orphan();
			}
		}
	}
}
