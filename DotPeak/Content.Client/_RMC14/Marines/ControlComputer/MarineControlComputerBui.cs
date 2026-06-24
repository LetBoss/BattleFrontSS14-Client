// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Marines.ControlComputer.MarineControlComputerBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Marines.ControlComputer;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Marines.ControlComputer;

public sealed class MarineControlComputerBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private MarineControlComputerWindow? _window;
  private bool _confirmingEvacuation;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<MarineControlComputerWindow>((BoundUserInterface) this);
    this.Refresh();
    ((BaseButton) this._window.AlertButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new MarineControlComputerAlertLevelMsg()));
    ((BaseButton) this._window.ShipAnnouncementButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new MarineControlComputerShipAnnouncementMsg()));
    ((BaseButton) this._window.MedalButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new MarineControlComputerMedalMsg()));
    ((BaseButton) this._window.EvacuationButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      if (this._confirmingEvacuation)
      {
        this.SendPredictedMessage((BoundUserInterfaceMessage) new MarineControlComputerToggleEvacuationMsg());
        this._confirmingEvacuation = false;
      }
      else
        this._confirmingEvacuation = true;
      this.Refresh();
    });
  }

  public void Refresh()
  {
    MarineControlComputerWindow window = this._window;
    MarineControlComputerComponent computerComponent;
    if (window == null || !((BaseWindow) window).IsOpen || !this.EntMan.TryGetComponent<MarineControlComputerComponent>(this.Owner, ref computerComponent))
      return;
    this._window.EvacuationButton.Text = !this._confirmingEvacuation ? (computerComponent.Evacuating ? "Cancel Evacuation" : "Initiate Evacuation") : "Confirm?";
    ((BaseButton) this._window.EvacuationButton).Disabled = !computerComponent.CanEvacuate;
  }
}
