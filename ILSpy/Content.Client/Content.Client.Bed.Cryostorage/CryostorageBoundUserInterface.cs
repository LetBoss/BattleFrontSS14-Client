using System;
using Content.Shared.Bed.Cryostorage;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Bed.Cryostorage;

public sealed class CryostorageBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private CryostorageMenu? _menu;

	public CryostorageBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<CryostorageMenu>((BoundUserInterface)(object)this);
		_menu.SlotRemoveButtonPressed += delegate(NetEntity ent, string slot)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CryostorageRemoveItemBuiMessage(ent, slot, CryostorageRemoveItemBuiMessage.RemovalType.Inventory));
		};
		_menu.HandRemoveButtonPressed += delegate(NetEntity ent, string hand)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CryostorageRemoveItemBuiMessage(ent, hand, CryostorageRemoveItemBuiMessage.RemovalType.Hand));
		};
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is CryostorageBuiState state2)
		{
			_menu?.UpdateState(state2);
		}
	}
}
