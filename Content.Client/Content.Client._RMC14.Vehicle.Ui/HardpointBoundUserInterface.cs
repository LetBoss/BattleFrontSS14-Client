using System;
using Content.Shared._RMC14.Vehicle;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Vehicle.Ui;

public sealed class HardpointBoundUserInterface : BoundUserInterface
{
	private HardpointMenu? _menu;

	public HardpointBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_menu = new HardpointMenu();
		((BaseWindow)_menu).OnClose += base.Close;
		_menu.VehicleEntity = ((BoundUserInterface)this).Owner;
		MetaDataComponent val = default(MetaDataComponent);
		if (base.EntMan.GetEntityQuery<MetaDataComponent>().TryGetComponent(((BoundUserInterface)this).Owner, ref val))
		{
			_menu.Title = val.EntityName;
		}
		_menu.OnRemove += delegate(string slotId)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new HardpointRemoveMessage(slotId));
		};
		((BaseWindow)_menu).OpenCentered();
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			if (_menu != null)
			{
				((BaseWindow)_menu).OnClose -= base.Close;
			}
			HardpointMenu? menu = _menu;
			if (menu != null)
			{
				((Control)menu).Dispose();
			}
			_menu = null;
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is HardpointBoundUserInterfaceState hardpointBoundUserInterfaceState)
		{
			_menu?.Update(hardpointBoundUserInterfaceState.Hardpoints, hardpointBoundUserInterfaceState.FrameIntegrity, hardpointBoundUserInterfaceState.FrameMaxIntegrity, hardpointBoundUserInterfaceState.HasFrameIntegrity, hardpointBoundUserInterfaceState.Error);
		}
	}
}
