using System;
using Content.Shared.Arcade;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Arcade.UI;

public sealed class SpaceVillainArcadeBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private SpaceVillainArcadeMenu? _menu;

	public SpaceVillainArcadeBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SendAction(SharedSpaceVillainArcadeComponent.PlayerAction.RequestData);
	}

	public void SendAction(SharedSpaceVillainArcadeComponent.PlayerAction action)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SharedSpaceVillainArcadeComponent.SpaceVillainArcadePlayerActionMessage(action));
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<SpaceVillainArcadeMenu>((BoundUserInterface)(object)this);
		_menu.OnPlayerAction += SendAction;
	}

	protected override void ReceiveMessage(BoundUserInterfaceMessage message)
	{
		if (message is SharedSpaceVillainArcadeComponent.SpaceVillainArcadeDataUpdateMessage message2)
		{
			_menu?.UpdateInfo(message2);
		}
	}
}
