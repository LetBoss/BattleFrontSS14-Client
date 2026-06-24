// Decompiled with JetBrains decompiler
// Type: Content.Shared.Robotics.CyborgControlData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Robotics;

[DataRecord]
[NetSerializable]
[Serializable]
public record struct CyborgControlData
{
  [DataField(null, false, 1, true, false, null)]
  public SpriteSpecifier? ChassisSprite;
  [DataField(null, false, 1, true, false, null)]
  public string ChassisName;
  [DataField(null, false, 1, true, false, null)]
  public string Name;
  [DataField(null, false, 1, false, false, null)]
  public float Charge;
  [DataField(null, false, 1, false, false, null)]
  public int ModuleCount;
  [DataField(null, false, 1, false, false, null)]
  public bool HasBrain;
  [DataField(null, false, 1, false, false, null)]
  public bool CanDisable;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  public TimeSpan Timeout;

  public CyborgControlData(
    SpriteSpecifier? chassisSprite,
    string chassisName,
    string name,
    float charge,
    int moduleCount,
    bool hasBrain,
    bool canDisable)
  {
    this.ChassisName = string.Empty;
    this.Name = string.Empty;
    this.Timeout = TimeSpan.Zero;
    this.ChassisSprite = chassisSprite;
    this.ChassisName = chassisName;
    this.Name = name;
    this.Charge = charge;
    this.ModuleCount = moduleCount;
    this.HasBrain = hasBrain;
    this.CanDisable = canDisable;
  }

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return ((((((EqualityComparer<SpriteSpecifier>.Default.GetHashCode(this.ChassisSprite) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.ChassisName)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Name)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.Charge)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.ModuleCount)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.HasBrain)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.CanDisable)) * -1521134295 + EqualityComparer<TimeSpan>.Default.GetHashCode(this.Timeout);
  }

  [CompilerGenerated]
  public readonly bool Equals(CyborgControlData other)
  {
    return EqualityComparer<SpriteSpecifier>.Default.Equals(this.ChassisSprite, other.ChassisSprite) && EqualityComparer<string>.Default.Equals(this.ChassisName, other.ChassisName) && EqualityComparer<string>.Default.Equals(this.Name, other.Name) && EqualityComparer<float>.Default.Equals(this.Charge, other.Charge) && EqualityComparer<int>.Default.Equals(this.ModuleCount, other.ModuleCount) && EqualityComparer<bool>.Default.Equals(this.HasBrain, other.HasBrain) && EqualityComparer<bool>.Default.Equals(this.CanDisable, other.CanDisable) && EqualityComparer<TimeSpan>.Default.Equals(this.Timeout, other.Timeout);
  }
}
