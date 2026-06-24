using System;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Nuke;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Nuke;

public sealed class NukeBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private NukeMenu? _menu;

	public NukeBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<NukeMenu>((BoundUserInterface)(object)this);
		_menu.OnKeypadButtonPressed += delegate(int i)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new NukeKeypadMessage(i));
		};
		_menu.OnEnterButtonPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new NukeKeypadEnterMessage());
		};
		_menu.OnClearButtonPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new NukeKeypadClearMessage());
		};
		((BaseButton)_menu.EjectButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ItemSlotButtonPressedEvent("Nuke"));
		};
		((BaseButton)_menu.AnchorButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new NukeAnchorMessage());
		};
		((BaseButton)_menu.ArmButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new NukeArmedMessage());
		};
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (_menu != null && state is NukeUiState state2)
		{
			_menu.UpdateState(state2);
		}
	}
}
