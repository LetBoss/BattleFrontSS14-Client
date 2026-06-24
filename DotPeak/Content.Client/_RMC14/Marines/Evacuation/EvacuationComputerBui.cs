// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Marines.Evacuation.EvacuationComputerBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Evacuation;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Marines.Evacuation;

public sealed class EvacuationComputerBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private EvacuationComputerWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<EvacuationComputerWindow>((BoundUserInterface) this);
    ((Control) this._window.DoorButton).Visible = false;
    ((BaseButton) this._window.EjectButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new EvacuationComputerLaunchBuiMsg()));
    ((Control) this._window.DelayButton).Visible = false;
    this.Refresh();
  }

  public void Refresh()
  {
    EvacuationComputerWindow window = this._window;
    EvacuationComputerComponent computerComponent;
    if (window == null || !((BaseWindow) window).IsOpen || !this.EntMan.TryGetComponent<EvacuationComputerComponent>(this.Owner, ref computerComponent))
      return;
    switch (computerComponent.Mode)
    {
      case EvacuationComputerMode.Disabled:
        this._window.StatusLabel.Text = "Escape Pod Status: DELAYED";
        this._window.HatchLabel.Text = "Docking Hatch: UNSECURED";
        break;
      case EvacuationComputerMode.Ready:
        this._window.StatusLabel.Text = "Escape Pod Status: STANDING BY";
        this._window.HatchLabel.Text = "Docking Hatch: SECURED";
        break;
      case EvacuationComputerMode.Travelling:
        this._window.StatusLabel.Text = "Escape Pod Status: TRAVELLING";
        this._window.HatchLabel.Text = "Docking Hatch: SECURED";
        break;
      case EvacuationComputerMode.Crashed:
        this._window.StatusLabel.Text = "Escape Pod Status: CRASHED";
        this._window.HatchLabel.Text = "Docking Hatch: UNSECURED";
        break;
    }
  }
}
