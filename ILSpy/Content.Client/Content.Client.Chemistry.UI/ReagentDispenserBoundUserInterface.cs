using System;
using Content.Client.UserInterface.Controls;
using Content.Shared.Chemistry;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Storage;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Chemistry.UI;

public sealed class ReagentDispenserBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private ReagentDispenserWindow? _window;

	public ReagentDispenserBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<ReagentDispenserWindow>((BoundUserInterface)(object)this);
		_window.SetInfoFromEntity(base.EntMan, ((BoundUserInterface)this).Owner);
		((BaseButton)_window.EjectButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ItemSlotButtonPressedEvent("beakerSlot"));
		};
		((BaseButton)_window.ClearButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ReagentDispenserClearContainerSolutionMessage());
		};
		ButtonGrid amountGrid = _window.AmountGrid;
		amountGrid.OnButtonPressed = (Action<string>)Delegate.Combine(amountGrid.OnButtonPressed, (Action<string>)delegate(string s)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ReagentDispenserSetDispenseAmountMessage(s));
		});
		_window.OnDispenseReagentButtonPressed += delegate(ItemStorageLocation location)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ReagentDispenserDispenseReagentMessage(location));
		};
		_window.OnEjectJugButtonPressed += delegate(ItemStorageLocation location)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ReagentDispenserEjectContainerMessage(location));
		};
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		ReagentDispenserBoundUserInterfaceState state2 = (ReagentDispenserBoundUserInterfaceState)(object)state;
		_window?.UpdateState((BoundUserInterfaceState)(object)state2);
	}
}
