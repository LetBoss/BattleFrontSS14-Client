using System;
using Content.Shared.Research.Components;
using Content.Shared.Research.Prototypes;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Research.UI;

public sealed class ResearchConsoleBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private ResearchConsoleMenu? _consoleMenu;

	public ResearchConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		EntityUid owner = ((BoundUserInterface)this).Owner;
		_consoleMenu = BoundUserInterfaceExt.CreateWindow<ResearchConsoleMenu>((BoundUserInterface)(object)this);
		_consoleMenu.SetEntity(owner);
		ResearchConsoleMenu? consoleMenu = _consoleMenu;
		consoleMenu.OnTechnologyCardPressed = (Action<string>)Delegate.Combine(consoleMenu.OnTechnologyCardPressed, (Action<string>)delegate(string id)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ConsoleUnlockTechnologyMessage(id));
		});
		ResearchConsoleMenu? consoleMenu2 = _consoleMenu;
		consoleMenu2.OnServerButtonPressed = (Action)Delegate.Combine(consoleMenu2.OnServerButtonPressed, (Action)delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ConsoleServerSelectionMessage());
		});
	}

	public override void OnProtoReload(PrototypesReloadedEventArgs args)
	{
		((BoundUserInterface)this).OnProtoReload(args);
		if (args.WasModified<TechnologyPrototype>() && ((BoundUserInterface)this).State is ResearchConsoleBoundInterfaceState state)
		{
			_consoleMenu?.UpdatePanels(state);
			_consoleMenu?.UpdateInformationPanel(state);
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is ResearchConsoleBoundInterfaceState state2)
		{
			_consoleMenu?.UpdatePanels(state2);
			_consoleMenu?.UpdateInformationPanel(state2);
		}
	}
}
