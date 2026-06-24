using System;
using Content.Shared.Research.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Research.UI;

public sealed class ResearchClientBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private ResearchClientServerSelectionMenu? _menu;

	public ResearchClientBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ResearchClientSyncMessage());
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<ResearchClientServerSelectionMenu>((BoundUserInterface)(object)this);
		_menu.OnServerSelected += SelectServer;
		_menu.OnServerDeselected += DeselectServer;
	}

	public void SelectServer(int serverId)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ResearchClientServerSelectedMessage(serverId));
	}

	public void DeselectServer()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ResearchClientServerDeselectedMessage());
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is ResearchClientBoundInterfaceState researchClientBoundInterfaceState)
		{
			_menu?.Populate(researchClientBoundInterfaceState.ServerCount, researchClientBoundInterfaceState.ServerNames, researchClientBoundInterfaceState.ServerIds, researchClientBoundInterfaceState.SelectedServerId);
		}
	}
}
