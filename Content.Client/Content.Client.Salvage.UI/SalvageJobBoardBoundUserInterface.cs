using System;
using Content.Shared.Salvage.JobBoard;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Salvage.UI;

public sealed class SalvageJobBoardBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private SalvageJobBoardMenu? _menu;

	public SalvageJobBoardBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<SalvageJobBoardMenu>((BoundUserInterface)(object)this);
		SalvageJobBoardMenu? menu = _menu;
		menu.OnLabelButtonPressed = (Action<string>)Delegate.Combine(menu.OnLabelButtonPressed, (Action<string>)delegate(string id)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new JobBoardPrintLabelMessage(id));
		});
	}

	protected override void UpdateState(BoundUserInterfaceState message)
	{
		((BoundUserInterface)this).UpdateState(message);
		if (message is SalvageJobBoardConsoleState state)
		{
			_menu?.Update(state);
		}
	}
}
