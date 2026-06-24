using System;
using Content.Shared.Chemistry;
using Content.Shared.Containers.ItemSlots;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Chemistry.UI;

public sealed class ChemMasterBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private ChemMasterWindow? _window;

	public ChemMasterBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<ChemMasterWindow>((BoundUserInterface)(object)this);
		_window.Title = base.EntMan.GetComponent<MetaDataComponent>(((BoundUserInterface)this).Owner).EntityName;
		((BaseButton)_window.InputEjectButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ItemSlotButtonPressedEvent("beakerSlot"));
		};
		((BaseButton)_window.OutputEjectButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ItemSlotButtonPressedEvent("outputSlot"));
		};
		((BaseButton)_window.BufferTransferButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ChemMasterSetModeMessage(ChemMasterMode.Transfer));
		};
		((BaseButton)_window.BufferDiscardButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ChemMasterSetModeMessage(ChemMasterMode.Discard));
		};
		((BaseButton)_window.CreatePillButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ChemMasterCreatePillsMessage((uint)_window.PillDosage.Value, (uint)_window.PillNumber.Value, _window.LabelLine));
		};
		((BaseButton)_window.CreateBottleButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ChemMasterOutputToBottleMessage((uint)_window.BottleDosage.Value, _window.LabelLine));
		};
		((BaseButton)_window.BufferSortButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ChemMasterSortingTypeCycleMessage());
		};
		for (uint num = 0u; num < _window.PillTypeButtons.Length; num++)
		{
			uint pillType = num;
			((BaseButton)_window.PillTypeButtons[num]).OnPressed += delegate
			{
				((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ChemMasterSetPillTypeMessage(pillType));
			};
		}
		_window.OnReagentButtonPressed += delegate(ButtonEventArgs args, ReagentButton button)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ChemMasterReagentAmountButtonMessage(button.Id, button.Amount, button.IsBuffer));
		};
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		ChemMasterBoundUserInterfaceState state2 = (ChemMasterBoundUserInterfaceState)(object)state;
		_window?.UpdateState((BoundUserInterfaceState)(object)state2);
	}
}
