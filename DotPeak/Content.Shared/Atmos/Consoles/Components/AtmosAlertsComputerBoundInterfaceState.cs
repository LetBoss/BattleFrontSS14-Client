// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Components.AtmosAlertsComputerBoundInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Atmos.Components;

[NetSerializable]
[Serializable]
public sealed class AtmosAlertsComputerBoundInterfaceState : BoundUserInterfaceState
{
  public AtmosAlertsComputerEntry[] AirAlarms;
  public AtmosAlertsComputerEntry[] FireAlarms;
  public AtmosAlertsFocusDeviceData? FocusData;

  public AtmosAlertsComputerBoundInterfaceState(
    AtmosAlertsComputerEntry[] airAlarms,
    AtmosAlertsComputerEntry[] fireAlarms,
    AtmosAlertsFocusDeviceData? focusData)
  {
    this.AirAlarms = airAlarms;
    this.FireAlarms = fireAlarms;
    this.FocusData = focusData;
  }
}
