// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Monitor.Components.AirAlarmUpdateDeviceDataMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Atmos.Monitor.Components;

[NetSerializable]
[Serializable]
public sealed class AirAlarmUpdateDeviceDataMessage : BoundUserInterfaceMessage
{
  public string Address { get; }

  public IAtmosDeviceData Data { get; }

  public AirAlarmUpdateDeviceDataMessage(string addr, IAtmosDeviceData data)
  {
    this.Address = addr;
    this.Data = data;
  }
}
