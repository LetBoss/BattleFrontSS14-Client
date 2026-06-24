using System;
using System.Numerics;
using Content.Shared._RMC14.Vehicle;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Vehicle.Ui;

public sealed class VehicleWeaponsBoundUserInterface : BoundUserInterface
{
	private VehicleWeaponsMenu? _menu;

	public VehicleWeaponsBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = new VehicleWeaponsMenu();
		((BaseWindow)_menu).OnClose += base.Close;
		_menu.Title = string.Empty;
		_menu.OnSelect += delegate(NetEntity? mountedEntity)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new VehicleWeaponsSelectMessage(mountedEntity));
		};
		_menu.OnToggleStabilization += delegate(bool enabled)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new VehicleWeaponsStabilizationMessage(enabled));
		};
		_menu.OnToggleAutoTurret += delegate(bool enabled)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new VehicleWeaponsAutoModeMessage(enabled));
		};
		((BaseWindow)_menu).OpenCenteredAt(new Vector2(0.7f, 0.05f));
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
			VehicleWeaponsMenu? menu = _menu;
			if (menu != null)
			{
				((Control)menu).Dispose();
			}
			_menu = null;
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).UpdateState(state);
		if (state is VehicleWeaponsUiState vehicleWeaponsUiState)
		{
			_menu?.Update(vehicleWeaponsUiState.Vehicle, vehicleWeaponsUiState.Hardpoints, vehicleWeaponsUiState.CanToggleStabilization, vehicleWeaponsUiState.StabilizationEnabled, vehicleWeaponsUiState.CanToggleAuto, vehicleWeaponsUiState.AutoEnabled);
		}
	}

	protected override void ReceiveMessage(BoundUserInterfaceMessage message)
	{
		((BoundUserInterface)this).ReceiveMessage(message);
		if (message is VehicleWeaponsCooldownFeedbackMessage vehicleWeaponsCooldownFeedbackMessage)
		{
			_menu?.FlashCooldownFeedback(vehicleWeaponsCooldownFeedbackMessage.RemainingSeconds);
		}
	}
}
