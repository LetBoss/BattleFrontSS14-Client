using System;
using System.Text;
using Content.Shared._RMC14.OrbitalCannon;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Client._RMC14.OrbitalCannon;

public sealed class OrbitalCannonComputerBui : BoundUserInterface
{
	[Dependency]
	private IPrototypeManager _prototype;

	private readonly ContainerSystem _container;

	private readonly OrbitalCannonSystem _orbitalCannon;

	private OrbitalCannonWindow? _window;

	public OrbitalCannonComputerBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_container = base.EntMan.System<ContainerSystem>();
		_orbitalCannon = base.EntMan.System<OrbitalCannonSystem>();
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<OrbitalCannonWindow>((BoundUserInterface)(object)this);
		Refresh();
	}

	public void Refresh()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		OrbitalCannonWindow window = _window;
		OrbitalCannonComputerComponent orbitalCannonComputerComponent = default(OrbitalCannonComputerComponent);
		if (window == null || !((BaseWindow)window).IsOpen || !base.EntMan.TryGetComponent<OrbitalCannonComputerComponent>(((BoundUserInterface)this).Owner, ref orbitalCannonComputerComponent))
		{
			return;
		}
		_window.WarheadStatusLabel.Text = Loc.GetString("rmc-ui-ob-warhead-none");
		if (orbitalCannonComputerComponent.Warhead != null)
		{
			_window.WarheadStatusLabel.Text = Loc.GetString("rmc-ui-ob-warhead-loaded", new(string, object)[1] { ("warhead", orbitalCannonComputerComponent.Warhead) });
		}
		_window.FuelStatusLabel.Text = Loc.GetString("rmc-ui-ob-fuel-count", new(string, object)[1] { ("count", orbitalCannonComputerComponent.Fuel) });
		StringBuilder stringBuilder = new StringBuilder();
		EntityPrototype val = default(EntityPrototype);
		foreach (WarheadFuelRequirement fuelRequirement in orbitalCannonComputerComponent.FuelRequirements)
		{
			if (_prototype.TryIndex(EntProtoId<OrbitalCannonWarheadComponent>.op_Implicit(fuelRequirement.Warhead), ref val))
			{
				stringBuilder.AppendLine(Loc.GetString("rmc-ui-ob-fuel-requirement", new(string, object)[2]
				{
					("warhead", val.Name),
					("fuel", fuelRequirement.Fuel)
				}));
			}
		}
		_window.FuelRequirementsLabel.Text = FormatRequirements(stringBuilder.ToString().Trim());
		if (_orbitalCannon.TryGetClosestCannon(((BoundUserInterface)this).Owner, out Entity<OrbitalCannonComponent> cannon))
		{
			((Range)_window.FuelProgressBar).Value = Math.Clamp(orbitalCannonComputerComponent.Fuel, 0, cannon.Comp.MaxFuel);
		}
		UpdateTrayControls(orbitalCannonComputerComponent);
	}

	private string FormatRequirements(string requirements)
	{
		if (string.IsNullOrEmpty(requirements))
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		string[] array = requirements.Split('\n');
		foreach (string value in array)
		{
			stringBuilder.Append("• ");
			stringBuilder.AppendLine(value);
		}
		return stringBuilder.ToString();
	}

	private void UpdateTrayControls(OrbitalCannonComputerComponent computer)
	{
		if (_window != null)
		{
			((BaseButton)_window.TrayButtonOne).OnPressed -= LoadTray;
			((BaseButton)_window.TrayButtonOne).OnPressed -= UnloadTray;
			((BaseButton)_window.TrayButtonOne).OnPressed -= ChamberTray;
			((BaseButton)_window.TrayButtonTwo).OnPressed -= LoadTray;
			((BaseButton)_window.TrayButtonTwo).OnPressed -= UnloadTray;
			((BaseButton)_window.TrayButtonTwo).OnPressed -= ChamberTray;
			switch (computer.Status)
			{
			case OrbitalCannonStatus.Unloaded:
				_window.TrayButtonOne.Text = Loc.GetString("rmc-ui-ob-load-tray");
				((Control)_window.TrayButtonOne).Visible = true;
				((BaseButton)_window.TrayButtonOne).OnPressed += LoadTray;
				((Control)_window.TrayButtonTwo).Visible = false;
				((Control)_window.TrayButtonLabel).Visible = false;
				((BaseButton)_window.TrayButtonOne).Disabled = computer.Warhead == null || computer.Fuel == 0;
				break;
			case OrbitalCannonStatus.Loaded:
				_window.TrayButtonOne.Text = Loc.GetString("rmc-ui-ob-unload-tray");
				((Control)_window.TrayButtonOne).Visible = true;
				((BaseButton)_window.TrayButtonOne).OnPressed += UnloadTray;
				_window.TrayButtonTwo.Text = Loc.GetString("rmc-ui-ob-chamber-tray");
				((Control)_window.TrayButtonTwo).Visible = true;
				((BaseButton)_window.TrayButtonTwo).OnPressed += ChamberTray;
				((Control)_window.TrayButtonLabel).Visible = false;
				break;
			case OrbitalCannonStatus.Chambered:
				((Control)_window.TrayButtonOne).Visible = false;
				((Control)_window.TrayButtonTwo).Visible = false;
				_window.TrayButtonLabel.Text = Loc.GetString("rmc-ui-ob-tray-chambered");
				((Control)_window.TrayButtonLabel).Visible = true;
				break;
			}
		}
	}

	private void LoadTray(ButtonEventArgs args)
	{
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OrbitalCannonComputerLoadBuiMsg());
	}

	private void UnloadTray(ButtonEventArgs args)
	{
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OrbitalCannonComputerUnloadBuiMsg());
	}

	private void ChamberTray(ButtonEventArgs args)
	{
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new OrbitalCannonComputerChamberBuiMsg());
	}
}
