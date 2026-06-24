using System;
using Content.Shared._RMC14.Evacuation;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Marines.Evacuation;

public sealed class EvacuationComputerBui : BoundUserInterface
{
	private EvacuationComputerWindow? _window;

	public EvacuationComputerBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<EvacuationComputerWindow>((BoundUserInterface)(object)this);
		((Control)_window.DoorButton).Visible = false;
		((BaseButton)_window.EjectButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new EvacuationComputerLaunchBuiMsg());
		};
		((Control)_window.DelayButton).Visible = false;
		Refresh();
	}

	public void Refresh()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EvacuationComputerWindow window = _window;
		EvacuationComputerComponent evacuationComputerComponent = default(EvacuationComputerComponent);
		if (window != null && ((BaseWindow)window).IsOpen && base.EntMan.TryGetComponent<EvacuationComputerComponent>(((BoundUserInterface)this).Owner, ref evacuationComputerComponent))
		{
			switch (evacuationComputerComponent.Mode)
			{
			case EvacuationComputerMode.Disabled:
				_window.StatusLabel.Text = "Escape Pod Status: DELAYED";
				_window.HatchLabel.Text = "Docking Hatch: UNSECURED";
				break;
			case EvacuationComputerMode.Ready:
				_window.StatusLabel.Text = "Escape Pod Status: STANDING BY";
				_window.HatchLabel.Text = "Docking Hatch: SECURED";
				break;
			case EvacuationComputerMode.Travelling:
				_window.StatusLabel.Text = "Escape Pod Status: TRAVELLING";
				_window.HatchLabel.Text = "Docking Hatch: SECURED";
				break;
			case EvacuationComputerMode.Crashed:
				_window.StatusLabel.Text = "Escape Pod Status: CRASHED";
				_window.HatchLabel.Text = "Docking Hatch: UNSECURED";
				break;
			}
		}
	}
}
