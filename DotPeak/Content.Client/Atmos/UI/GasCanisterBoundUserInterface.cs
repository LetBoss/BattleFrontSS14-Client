// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.UI.GasCanisterBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.IdentityManagement;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.IdentityManagement;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Atmos.UI;

public sealed class GasCanisterBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private GasCanisterWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<GasCanisterWindow>((BoundUserInterface) this);
    this._window.ReleaseValveCloseButtonPressed += new Action(this.OnReleaseValveClosePressed);
    this._window.ReleaseValveOpenButtonPressed += new Action(this.OnReleaseValveOpenPressed);
    this._window.ReleasePressureSet += new Action<float>(this.OnReleasePressureSet);
    this._window.TankEjectButtonPressed += new Action(this.OnTankEjectPressed);
  }

  private void OnTankEjectPressed()
  {
    this.SendPredictedMessage((BoundUserInterfaceMessage) new GasCanisterHoldingTankEjectMessage());
  }

  private void OnReleasePressureSet(float value)
  {
    this.SendPredictedMessage((BoundUserInterfaceMessage) new GasCanisterChangeReleasePressureMessage(value));
  }

  private void OnReleaseValveOpenPressed()
  {
    this.SendPredictedMessage((BoundUserInterfaceMessage) new GasCanisterChangeReleaseValveMessage(true));
  }

  private void OnReleaseValveClosePressed()
  {
    this.SendPredictedMessage((BoundUserInterfaceMessage) new GasCanisterChangeReleaseValveMessage(false));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    GasCanisterComponent canisterComponent;
    if (this._window == null || !(state is GasCanisterBoundUserInterfaceState userInterfaceState) || !this.EntMan.TryGetComponent<GasCanisterComponent>(this.Owner, ref canisterComponent))
      return;
    IdentityEntity label1 = Identity.Name(this.Owner, this.EntMan);
    EntityUid? nullable = canisterComponent.GasTankSlot.Item;
    string str;
    if (!nullable.HasValue)
    {
      str = (string) null;
    }
    else
    {
      nullable = canisterComponent.GasTankSlot.Item;
      EntityUid uid = nullable.Value;
      IEntityManager entMan = this.EntMan;
      nullable = new EntityUid?();
      EntityUid? viewer = nullable;
      str = Identity.Name(uid, entMan, viewer).Name;
    }
    string label2 = str;
    this._window.SetCanisterLabel((string) label1);
    this._window.SetCanisterPressure(userInterfaceState.CanisterPressure);
    this._window.SetPortStatus(userInterfaceState.PortStatus);
    this._window.SetTankLabel(label2);
    this._window.SetTankPressure(userInterfaceState.TankPressure);
    this._window.SetReleasePressureRange(canisterComponent.MinReleasePressure, canisterComponent.MaxReleasePressure);
    this._window.SetReleasePressure(canisterComponent.ReleasePressure);
    this._window.SetReleaseValve(canisterComponent.ReleaseValve);
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    ((Control) this._window)?.Orphan();
  }
}
