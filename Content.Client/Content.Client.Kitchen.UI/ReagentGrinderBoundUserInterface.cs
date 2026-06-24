using System;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Kitchen;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Kitchen.UI;

public sealed class ReagentGrinderBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private GrinderMenu? _menu;

	public ReagentGrinderBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<GrinderMenu>((BoundUserInterface)(object)this);
		_menu.OnToggleAuto += ToggleAutoMode;
		_menu.OnGrind += StartGrinding;
		_menu.OnJuice += StartJuicing;
		_menu.OnEjectAll += EjectAll;
		_menu.OnEjectBeaker += EjectBeaker;
		_menu.OnEjectChamber += EjectChamberContent;
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is ReagentGrinderInterfaceState state2)
		{
			_menu?.UpdateState(state2);
		}
	}

	protected override void ReceiveMessage(BoundUserInterfaceMessage message)
	{
		((BoundUserInterface)this).ReceiveMessage(message);
		_menu?.HandleMessage(message);
	}

	public void ToggleAutoMode()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ReagentGrinderToggleAutoModeMessage());
	}

	public void StartGrinding()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ReagentGrinderStartMessage(GrinderProgram.Grind));
	}

	public void StartJuicing()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ReagentGrinderStartMessage(GrinderProgram.Juice));
	}

	public void EjectAll()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ReagentGrinderEjectChamberAllMessage());
	}

	public void EjectBeaker()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ItemSlotButtonPressedEvent(SharedReagentGrinder.BeakerSlotId));
	}

	public void EjectChamberContent(EntityUid uid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ReagentGrinderEjectChamberContentMessage(base.EntMan.GetNetEntity(uid, (MetaDataComponent)null)));
	}
}
