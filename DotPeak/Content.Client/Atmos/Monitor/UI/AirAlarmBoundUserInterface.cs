// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.Monitor.UI.AirAlarmBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Atmos;
using Content.Shared.Atmos.Monitor;
using Content.Shared.Atmos.Monitor.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Atmos.Monitor.UI;

public sealed class AirAlarmBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private AirAlarmWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<AirAlarmWindow>((BoundUserInterface) this);
    this._window.SetEntity(this.Owner);
    this._window.AtmosDeviceDataChanged += new Action<string, IAtmosDeviceData>(this.OnDeviceDataChanged);
    this._window.AtmosDeviceDataCopied += new Action<IAtmosDeviceData>(this.OnDeviceDataCopied);
    this._window.AtmosAlarmThresholdChanged += new Action<string, AtmosMonitorThresholdType, AtmosAlarmThreshold, Gas?>(this.OnThresholdChanged);
    this._window.AirAlarmModeChanged += new Action<AirAlarmMode>(this.OnAirAlarmModeChanged);
    this._window.AutoModeChanged += new Action<bool>(this.OnAutoModeChanged);
    this._window.ResyncAllRequested += new Action(this.ResyncAllDevices);
  }

  private void ResyncAllDevices()
  {
    this.SendMessage((BoundUserInterfaceMessage) new AirAlarmResyncAllDevicesMessage());
  }

  private void OnDeviceDataChanged(string address, IAtmosDeviceData data)
  {
    this.SendMessage((BoundUserInterfaceMessage) new AirAlarmUpdateDeviceDataMessage(address, data));
  }

  private void OnDeviceDataCopied(IAtmosDeviceData data)
  {
    this.SendMessage((BoundUserInterfaceMessage) new AirAlarmCopyDeviceDataMessage(data));
  }

  private void OnAirAlarmModeChanged(AirAlarmMode mode)
  {
    this.SendMessage((BoundUserInterfaceMessage) new AirAlarmUpdateAlarmModeMessage(mode));
  }

  private void OnAutoModeChanged(bool enabled)
  {
    this.SendMessage((BoundUserInterfaceMessage) new AirAlarmUpdateAutoModeMessage(enabled));
  }

  private void OnThresholdChanged(
    string address,
    AtmosMonitorThresholdType type,
    AtmosAlarmThreshold threshold,
    Gas? gas = null)
  {
    this.SendMessage((BoundUserInterfaceMessage) new AirAlarmUpdateAlarmThresholdMessage(address, type, threshold, gas));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is AirAlarmUIState state1) || this._window == null)
      return;
    this._window.UpdateState(state1);
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    ((Control) this._window)?.Orphan();
  }
}
