// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.UI.GasPressurePumpBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.IdentityManagement;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Atmos.UI;

public sealed class GasPressurePumpBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private GasPressurePumpWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<GasPressurePumpWindow>((BoundUserInterface) this);
    this._window.ToggleStatusButtonPressed += new Action(this.OnToggleStatusButtonPressed);
    this._window.PumpOutputPressureChanged += new Action<float>(this.OnPumpOutputPressurePressed);
    base.Update();
  }

  public virtual void Update()
  {
    if (this._window == null)
      return;
    this._window.Title = (string) Identity.Name(this.Owner, this.EntMan);
    GasPressurePumpComponent pressurePumpComponent;
    if (!this.EntMan.TryGetComponent<GasPressurePumpComponent>(this.Owner, ref pressurePumpComponent))
      return;
    this._window.SetPumpStatus(pressurePumpComponent.Enabled);
    this._window.MaxPressure = pressurePumpComponent.MaxTargetPressure;
    this._window.SetOutputPressure(pressurePumpComponent.TargetPressure);
  }

  private void OnToggleStatusButtonPressed()
  {
    if (this._window == null)
      return;
    this.SendPredictedMessage((BoundUserInterfaceMessage) new GasPressurePumpToggleStatusMessage(this._window.PumpStatus));
  }

  private void OnPumpOutputPressurePressed(float value)
  {
    this.SendPredictedMessage((BoundUserInterfaceMessage) new GasPressurePumpChangeOutputPressureMessage(value));
  }
}
