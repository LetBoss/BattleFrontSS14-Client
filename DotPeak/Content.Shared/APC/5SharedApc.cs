// Decompiled with JetBrains decompiler
// Type: Content.Shared.APC.ApcBoundInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.APC;

[NetSerializable]
[Serializable]
public sealed class ApcBoundInterfaceState : 
  BoundUserInterfaceState,
  IEquatable<ApcBoundInterfaceState>
{
  public readonly bool MainBreaker;
  public readonly int Power;
  public readonly ApcExternalPowerState ApcExternalPower;
  public readonly float Charge;

  public ApcBoundInterfaceState(
    bool mainBreaker,
    int power,
    ApcExternalPowerState apcExternalPower,
    float charge)
  {
    this.MainBreaker = mainBreaker;
    this.Power = power;
    this.ApcExternalPower = apcExternalPower;
    this.Charge = charge;
  }

  public bool Equals(ApcBoundInterfaceState? other)
  {
    if (other == null)
      return false;
    if (this == other)
      return true;
    return this.MainBreaker == other.MainBreaker && this.Power == other.Power && this.ApcExternalPower == other.ApcExternalPower && MathHelper.CloseTo(this.Charge, other.Charge, 1E-07f);
  }

  public virtual bool Equals(object? obj)
  {
    if (this == obj)
      return true;
    return obj is ApcBoundInterfaceState other && this.Equals(other);
  }

  public virtual int GetHashCode()
  {
    return HashCode.Combine<bool, int, int, float>(this.MainBreaker, this.Power, (int) this.ApcExternalPower, this.Charge);
  }
}
