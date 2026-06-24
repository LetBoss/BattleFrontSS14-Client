// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.SupplyDrop.SupplyDropComputerBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.SupplyDrop;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Client._RMC14.SupplyDrop;

public sealed class SupplyDropComputerBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private SupplyDropWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<SupplyDropWindow>((BoundUserInterface) this);
    this._window.Longitude.OnValueChanged += (Action<FloatSpinBox.FloatSpinBoxEventArgs>) (args => this.SendPredictedMessage((BoundUserInterfaceMessage) new SupplyDropComputerLongitudeBuiMsg((int) args.Value)));
    this._window.Latitude.OnValueChanged += (Action<FloatSpinBox.FloatSpinBoxEventArgs>) (args => this.SendPredictedMessage((BoundUserInterfaceMessage) new SupplyDropComputerLatitudeBuiMsg((int) args.Value)));
    ((BaseButton) this._window.LaunchButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new SupplyDropComputerLaunchBuiMsg()));
    ((BaseButton) this._window.UpdateButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new SupplyDropComputerUpdateBuiMsg()));
    this.Refresh();
  }

  public void Refresh()
  {
    SupplyDropWindow window = this._window;
    SupplyDropComputerComponent computerComponent;
    if (window == null || !((BaseWindow) window).IsOpen || !this.EntMan.TryGetComponent<SupplyDropComputerComponent>(this.Owner, ref computerComponent))
      return;
    this._window.Longitude.Value = (float) computerComponent.Coordinates.X;
    this._window.Latitude.Value = (float) computerComponent.Coordinates.Y;
    this._window.LastUpdateAt = computerComponent.LastLaunchAt;
    this._window.NextUpdateAt = computerComponent.NextLaunchAt;
    this._window.CrateStatusLabel.Text = Loc.GetString("ui-supply-drop-crate-status", new (string, object)[1]
    {
      ("hasCrate", (object) computerComponent.HasCrate)
    });
  }
}
