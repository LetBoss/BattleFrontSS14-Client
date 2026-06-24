// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Events.StaminaMeleeHitEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Components;
using Robust.Shared.GameObjects;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Damage.Events;

public sealed class StaminaMeleeHitEvent : HandledEntityEventArgs
{
  public List<(EntityUid Entity, StaminaComponent Component)> HitList;
  public float Multiplier = 1f;
  public float FlatModifier;

  public StaminaMeleeHitEvent(
    List<(EntityUid Entity, StaminaComponent Component)> hitList)
  {
    this.HitList = hitList;
  }
}
