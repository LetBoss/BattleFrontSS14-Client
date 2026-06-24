// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.DamageableComponentState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Damage;

[NetSerializable]
[Serializable]
public sealed class DamageableComponentState : ComponentState
{
  public readonly Dictionary<string, FixedPoint2> DamageDict;
  public readonly string? DamageContainerId;
  public readonly string? ModifierSetId;
  public readonly FixedPoint2? HealthBarThreshold;

  public DamageableComponentState(
    Dictionary<string, FixedPoint2> damageDict,
    string? damageContainerId,
    string? modifierSetId,
    FixedPoint2? healthBarThreshold)
  {
    this.DamageDict = damageDict;
    this.DamageContainerId = damageContainerId;
    this.ModifierSetId = modifierSetId;
    this.HealthBarThreshold = healthBarThreshold;
  }
}
