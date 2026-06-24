// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.UI.GasPressureRegulatorBoundUserInterface
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

public sealed class GasPressureRegulatorBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  private GasPressureRegulatorWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<GasPressureRegulatorWindow>((BoundUserInterface) this);
    this._window.SetEntity(this.Owner);
    this._window.ThresholdPressureChanged += new Action<string>(this.OnThresholdChanged);
    GasPressureRegulatorComponent regulatorComponent;
    if (this.EntMan.TryGetComponent<GasPressureRegulatorComponent>(this.Owner, ref regulatorComponent))
      this._window.SetThresholdPressureInput(regulatorComponent.Threshold);
    base.Update();
  }

  public virtual void Update()
  {
    if (this._window == null)
      return;
    this._window.Title = (string) Identity.Name(this.Owner, this.EntMan);
    GasPressureRegulatorComponent regulatorComponent;
    if (!this.EntMan.TryGetComponent<GasPressureRegulatorComponent>(this.Owner, ref regulatorComponent))
      return;
    this._window.SetThresholdPressureLabel(regulatorComponent.Threshold);
    this._window.UpdateInfo(regulatorComponent.InletPressure, regulatorComponent.OutletPressure, regulatorComponent.FlowRate);
  }

  private void OnThresholdChanged(string newThreshold)
  {
    float num = 0.0f;
    float result;
    if (UserInputParser.TryFloat((ReadOnlySpan<char>) newThreshold, out result) && (double) result >= 0.0 && !float.IsInfinity(result))
      num = result;
    this._window?.SetThresholdPressureInput(num);
    this.SendPredictedMessage((BoundUserInterfaceMessage) new GasPressureRegulatorChangeThresholdMessage(num));
  }
}
