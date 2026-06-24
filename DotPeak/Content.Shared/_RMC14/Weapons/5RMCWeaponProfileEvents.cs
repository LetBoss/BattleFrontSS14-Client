// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.RMCWeaponViewProfileEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared._RMC14.Weapons;

[NetSerializable]
[Serializable]
public sealed class RMCWeaponViewProfileEvent : EntityEventArgs
{
  public int Nonce { get; }

  public bool ComponentDrawFov { get; }

  public bool RuntimeDrawFov { get; }

  public bool ComponentDrawLight { get; }

  public bool RuntimeDrawLight { get; }

  public bool ExaminerSkipChecks { get; }

  public bool ExaminerCheckInRangeUnOccluded { get; }

  public RMCWeaponViewProfileEvent(
    int nonce,
    bool componentDrawFov,
    bool runtimeDrawFov,
    bool componentDrawLight,
    bool runtimeDrawLight,
    bool examinerSkipChecks,
    bool examinerCheckInRangeUnOccluded)
  {
    this.Nonce = nonce;
    this.ComponentDrawFov = componentDrawFov;
    this.RuntimeDrawFov = runtimeDrawFov;
    this.ComponentDrawLight = componentDrawLight;
    this.RuntimeDrawLight = runtimeDrawLight;
    this.ExaminerSkipChecks = examinerSkipChecks;
    this.ExaminerCheckInRangeUnOccluded = examinerCheckInRangeUnOccluded;
  }
}
