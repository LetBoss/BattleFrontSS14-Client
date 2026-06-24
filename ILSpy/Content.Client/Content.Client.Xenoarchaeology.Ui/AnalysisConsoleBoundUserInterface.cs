using System;
using Content.Shared.Research.Components;
using Content.Shared.Xenoarchaeology.Equipment.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Xenoarchaeology.Ui;

public sealed class AnalysisConsoleBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private AnalysisConsoleMenu? _consoleMenu;

	public AnalysisConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_consoleMenu = BoundUserInterfaceExt.CreateWindow<AnalysisConsoleMenu>((BoundUserInterface)(object)this);
		_consoleMenu.SetOwner(((BoundUserInterface)this).Owner);
		((BaseWindow)_consoleMenu).OnClose += base.Close;
		((BaseWindow)_consoleMenu).OpenCentered();
		_consoleMenu.OnServerSelectionButtonPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ConsoleServerSelectionMessage());
		};
		_consoleMenu.OnExtractButtonPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new AnalysisConsoleExtractButtonPressedMessage());
		};
	}

	public void Update(Entity<AnalysisConsoleComponent> ent)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		_consoleMenu?.Update(ent);
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			AnalysisConsoleMenu? consoleMenu = _consoleMenu;
			if (consoleMenu != null)
			{
				((Control)consoleMenu).Orphan();
			}
		}
	}
}
