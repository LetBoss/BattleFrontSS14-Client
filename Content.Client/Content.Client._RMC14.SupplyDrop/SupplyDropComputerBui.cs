using System;
using Content.Shared._RMC14.SupplyDrop;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Client._RMC14.SupplyDrop;

public sealed class SupplyDropComputerBui : BoundUserInterface
{
	private SupplyDropWindow? _window;

	public SupplyDropComputerBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<SupplyDropWindow>((BoundUserInterface)(object)this);
		_window.Longitude.OnValueChanged += delegate(FloatSpinBoxEventArgs args)
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new SupplyDropComputerLongitudeBuiMsg((int)args.Value));
		};
		_window.Latitude.OnValueChanged += delegate(FloatSpinBoxEventArgs args)
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new SupplyDropComputerLatitudeBuiMsg((int)args.Value));
		};
		((BaseButton)_window.LaunchButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new SupplyDropComputerLaunchBuiMsg());
		};
		((BaseButton)_window.UpdateButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new SupplyDropComputerUpdateBuiMsg());
		};
		Refresh();
	}

	public void Refresh()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		SupplyDropWindow window = _window;
		SupplyDropComputerComponent supplyDropComputerComponent = default(SupplyDropComputerComponent);
		if (window != null && ((BaseWindow)window).IsOpen && base.EntMan.TryGetComponent<SupplyDropComputerComponent>(((BoundUserInterface)this).Owner, ref supplyDropComputerComponent))
		{
			_window.Longitude.Value = supplyDropComputerComponent.Coordinates.X;
			_window.Latitude.Value = supplyDropComputerComponent.Coordinates.Y;
			_window.LastUpdateAt = supplyDropComputerComponent.LastLaunchAt;
			_window.NextUpdateAt = supplyDropComputerComponent.NextLaunchAt;
			_window.CrateStatusLabel.Text = Loc.GetString("ui-supply-drop-crate-status", new(string, object)[1] { ("hasCrate", supplyDropComputerComponent.HasCrate) });
		}
	}
}
