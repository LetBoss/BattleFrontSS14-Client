// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.UI.GasVolumePumpBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Localizations;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Atmos.UI;

public sealed class GasVolumePumpBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private float _maxTransferRate;
  [Robust.Shared.ViewVariables.ViewVariables]
  private GasVolumePumpWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<GasVolumePumpWindow>((BoundUserInterface) this);
    GasVolumePumpComponent volumePumpComponent;
    if (this.EntMan.TryGetComponent<GasVolumePumpComponent>(this.Owner, ref volumePumpComponent))
      this._maxTransferRate = volumePumpComponent.MaxTransferRate;
    this._window.ToggleStatusButtonPressed += new Action(this.OnToggleStatusButtonPressed);
    this._window.PumpTransferRateChanged += new Action<string>(this.OnPumpTransferRatePressed);
    base.Update();
  }

  private void OnToggleStatusButtonPressed()
  {
    if (this._window == null)
      return;
    this.SendPredictedMessage((BoundUserInterfaceMessage) new GasVolumePumpToggleStatusMessage(this._window.PumpStatus));
  }

  private void OnPumpTransferRatePressed(string value)
  {
    float result;
    this.SendPredictedMessage((BoundUserInterfaceMessage) new GasVolumePumpChangeTransferRateMessage(Math.Clamp(UserInputParser.TryFloat((ReadOnlySpan<char>) value, out result) ? result : 0.0f, 0.0f, this._maxTransferRate)));
  }

  public virtual void Update()
  {
    base.Update();
    GasVolumePumpComponent volumePumpComponent;
    if (this._window == null || !this.EntMan.TryGetComponent<GasVolumePumpComponent>(this.Owner, ref volumePumpComponent))
      return;
    this._window.Title = (string) Identity.Name(this.Owner, this.EntMan);
    this._window.SetPumpStatus(volumePumpComponent.Enabled);
    this._window.SetTransferRate(volumePumpComponent.TransferRate);
  }
}
