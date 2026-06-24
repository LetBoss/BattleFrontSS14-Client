// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.OrbitalCannon.OrbitalCannonComputerBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.OrbitalCannon;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using System;
using System.Text;

#nullable enable
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
    this._container = this.EntMan.System<ContainerSystem>();
    this._orbitalCannon = this.EntMan.System<OrbitalCannonSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<OrbitalCannonWindow>((BoundUserInterface) this);
    this.Refresh();
  }

  public void Refresh()
  {
    OrbitalCannonWindow window = this._window;
    OrbitalCannonComputerComponent computer;
    if (window == null || !((BaseWindow) window).IsOpen || !this.EntMan.TryGetComponent<OrbitalCannonComputerComponent>(this.Owner, ref computer))
      return;
    this._window.WarheadStatusLabel.Text = Loc.GetString("rmc-ui-ob-warhead-none");
    if (computer.Warhead != null)
      this._window.WarheadStatusLabel.Text = Loc.GetString("rmc-ui-ob-warhead-loaded", new (string, object)[1]
      {
        ("warhead", (object) computer.Warhead)
      });
    this._window.FuelStatusLabel.Text = Loc.GetString("rmc-ui-ob-fuel-count", new (string, object)[1]
    {
      ("count", (object) computer.Fuel)
    });
    StringBuilder stringBuilder = new StringBuilder();
    foreach (WarheadFuelRequirement fuelRequirement in computer.FuelRequirements)
    {
      EntityPrototype entityPrototype;
      if (this._prototype.TryIndex(EntProtoId<OrbitalCannonWarheadComponent>.op_Implicit(fuelRequirement.Warhead), ref entityPrototype))
        stringBuilder.AppendLine(Loc.GetString("rmc-ui-ob-fuel-requirement", new (string, object)[2]
        {
          ("warhead", (object) entityPrototype.Name),
          ("fuel", (object) fuelRequirement.Fuel)
        }));
    }
    this._window.FuelRequirementsLabel.Text = this.FormatRequirements(stringBuilder.ToString().Trim());
    Entity<OrbitalCannonComponent> cannon;
    if (this._orbitalCannon.TryGetClosestCannon(this.Owner, out cannon))
      ((Range) this._window.FuelProgressBar).Value = (float) Math.Clamp(computer.Fuel, 0, cannon.Comp.MaxFuel);
    this.UpdateTrayControls(computer);
  }

  private string FormatRequirements(string requirements)
  {
    if (string.IsNullOrEmpty(requirements))
      return string.Empty;
    StringBuilder stringBuilder = new StringBuilder();
    foreach (string str in requirements.Split('\n'))
    {
      stringBuilder.Append("• ");
      stringBuilder.AppendLine(str);
    }
    return stringBuilder.ToString();
  }

  private void UpdateTrayControls(OrbitalCannonComputerComponent computer)
  {
    if (this._window == null)
      return;
    ((BaseButton) this._window.TrayButtonOne).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.LoadTray);
    ((BaseButton) this._window.TrayButtonOne).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.UnloadTray);
    ((BaseButton) this._window.TrayButtonOne).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.ChamberTray);
    ((BaseButton) this._window.TrayButtonTwo).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.LoadTray);
    ((BaseButton) this._window.TrayButtonTwo).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.UnloadTray);
    ((BaseButton) this._window.TrayButtonTwo).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.ChamberTray);
    switch (computer.Status)
    {
      case OrbitalCannonStatus.Unloaded:
        this._window.TrayButtonOne.Text = Loc.GetString("rmc-ui-ob-load-tray");
        ((Control) this._window.TrayButtonOne).Visible = true;
        ((BaseButton) this._window.TrayButtonOne).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.LoadTray);
        ((Control) this._window.TrayButtonTwo).Visible = false;
        ((Control) this._window.TrayButtonLabel).Visible = false;
        ((BaseButton) this._window.TrayButtonOne).Disabled = computer.Warhead == null || computer.Fuel == 0;
        break;
      case OrbitalCannonStatus.Loaded:
        this._window.TrayButtonOne.Text = Loc.GetString("rmc-ui-ob-unload-tray");
        ((Control) this._window.TrayButtonOne).Visible = true;
        ((BaseButton) this._window.TrayButtonOne).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.UnloadTray);
        this._window.TrayButtonTwo.Text = Loc.GetString("rmc-ui-ob-chamber-tray");
        ((Control) this._window.TrayButtonTwo).Visible = true;
        ((BaseButton) this._window.TrayButtonTwo).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.ChamberTray);
        ((Control) this._window.TrayButtonLabel).Visible = false;
        break;
      case OrbitalCannonStatus.Chambered:
        ((Control) this._window.TrayButtonOne).Visible = false;
        ((Control) this._window.TrayButtonTwo).Visible = false;
        this._window.TrayButtonLabel.Text = Loc.GetString("rmc-ui-ob-tray-chambered");
        ((Control) this._window.TrayButtonLabel).Visible = true;
        break;
    }
  }

  private void LoadTray(BaseButton.ButtonEventArgs args)
  {
    this.SendPredictedMessage((BoundUserInterfaceMessage) new OrbitalCannonComputerLoadBuiMsg());
  }

  private void UnloadTray(BaseButton.ButtonEventArgs args)
  {
    this.SendPredictedMessage((BoundUserInterfaceMessage) new OrbitalCannonComputerUnloadBuiMsg());
  }

  private void ChamberTray(BaseButton.ButtonEventArgs args)
  {
    this.SendPredictedMessage((BoundUserInterfaceMessage) new OrbitalCannonComputerChamberBuiMsg());
  }
}
