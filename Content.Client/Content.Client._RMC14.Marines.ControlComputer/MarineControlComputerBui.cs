using System;
using Content.Shared._RMC14.Marines.ControlComputer;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Marines.ControlComputer;

public sealed class MarineControlComputerBui : BoundUserInterface
{
	private MarineControlComputerWindow? _window;

	private bool _confirmingEvacuation;

	public MarineControlComputerBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<MarineControlComputerWindow>((BoundUserInterface)(object)this);
		Refresh();
		((BaseButton)_window.AlertButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new MarineControlComputerAlertLevelMsg());
		};
		((BaseButton)_window.ShipAnnouncementButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new MarineControlComputerShipAnnouncementMsg());
		};
		((BaseButton)_window.MedalButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new MarineControlComputerMedalMsg());
		};
		((BaseButton)_window.EvacuationButton).OnPressed += delegate
		{
			if (_confirmingEvacuation)
			{
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new MarineControlComputerToggleEvacuationMsg());
				_confirmingEvacuation = false;
			}
			else
			{
				_confirmingEvacuation = true;
			}
			Refresh();
		};
	}

	public void Refresh()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		MarineControlComputerWindow window = _window;
		MarineControlComputerComponent marineControlComputerComponent = default(MarineControlComputerComponent);
		if (window != null && ((BaseWindow)window).IsOpen && base.EntMan.TryGetComponent<MarineControlComputerComponent>(((BoundUserInterface)this).Owner, ref marineControlComputerComponent))
		{
			if (_confirmingEvacuation)
			{
				_window.EvacuationButton.Text = "Confirm?";
			}
			else
			{
				_window.EvacuationButton.Text = (marineControlComputerComponent.Evacuating ? "Cancel Evacuation" : "Initiate Evacuation");
			}
			((BaseButton)_window.EvacuationButton).Disabled = !marineControlComputerComponent.CanEvacuate;
		}
	}
}
