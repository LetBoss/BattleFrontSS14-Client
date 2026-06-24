using System;
using Content.Shared.Silicons.Borgs;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Silicons.Borgs;

public sealed class BorgBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private BorgMenu? _menu;

	public BorgBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<BorgMenu>((BoundUserInterface)(object)this);
		_menu.SetEntity(((BoundUserInterface)this).Owner);
		BorgMenu? menu = _menu;
		menu.BrainButtonPressed = (Action)Delegate.Combine(menu.BrainButtonPressed, (Action)delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new BorgEjectBrainBuiMessage());
		});
		BorgMenu? menu2 = _menu;
		menu2.EjectBatteryButtonPressed = (Action)Delegate.Combine(menu2.EjectBatteryButtonPressed, (Action)delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new BorgEjectBatteryBuiMessage());
		});
		BorgMenu? menu3 = _menu;
		menu3.NameChanged = (Action<string>)Delegate.Combine(menu3.NameChanged, (Action<string>)delegate(string name)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new BorgSetNameBuiMessage(name));
		});
		BorgMenu? menu4 = _menu;
		menu4.RemoveModuleButtonPressed = (Action<EntityUid>)Delegate.Combine(menu4.RemoveModuleButtonPressed, (Action<EntityUid>)delegate(EntityUid module)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new BorgRemoveModuleBuiMessage(base.EntMan.GetNetEntity(module, (MetaDataComponent)null)));
		});
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is BorgBuiState state2)
		{
			_menu?.UpdateState(state2);
		}
	}
}
