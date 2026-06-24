// Decompiled with JetBrains decompiler
// Type: Content.Shared.Power.PowerChargeState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.Power;

[NetSerializable]
[Serializable]
public sealed class PowerChargeState : BoundUserInterfaceState
{
  public bool On;
  public byte Charge;
  public PowerChargePowerStatus PowerStatus;
  public short PowerDraw;
  public short PowerDrawMax;
  public short EtaSeconds;

  public PowerChargeState(
    bool on,
    byte charge,
    PowerChargePowerStatus powerStatus,
    short powerDraw,
    short powerDrawMax,
    short etaSeconds)
  {
    this.On = on;
    this.Charge = charge;
    this.PowerStatus = powerStatus;
    this.PowerDraw = powerDraw;
    this.PowerDrawMax = powerDrawMax;
    this.EtaSeconds = etaSeconds;
  }
}
