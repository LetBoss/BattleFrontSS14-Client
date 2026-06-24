using System;
using Content.Shared.Research;
using Content.Shared.Research.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Research.UI;

public sealed class DiskConsoleBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private DiskConsoleMenu? _menu;

	public DiskConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<DiskConsoleMenu>((BoundUserInterface)(object)this);
		_menu.OnServerButtonPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ConsoleServerSelectionMessage());
		};
		_menu.OnPrintButtonPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new DiskConsolePrintDiskMessage());
		};
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is DiskConsoleBoundUserInterfaceState state2)
		{
			_menu?.Update(state2);
		}
	}
}
