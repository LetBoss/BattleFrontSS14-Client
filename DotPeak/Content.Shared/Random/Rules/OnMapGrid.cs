// Decompiled with JetBrains decompiler
// Type: Content.Shared.Random.Rules.OnMapGridRule
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Random.Rules;

public sealed class OnMapGridRule : 
  RulesRule,
  ISerializationGenerated<OnMapGridRule>,
  ISerializationGenerated
{
  public override bool Check(EntityManager entManager, EntityUid uid)
  {
    TransformComponent component;
    if (entManager.TryGetComponent<TransformComponent>(uid, out component))
    {
      EntityUid? gridUid = component.GridUid;
      EntityUid? mapUid = component.MapUid;
      if ((gridUid.HasValue == mapUid.HasValue ? (gridUid.HasValue ? (gridUid.GetValueOrDefault() != mapUid.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0 && component.MapUid.HasValue)
        return !this.Inverted;
    }
    return this.Inverted;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref OnMapGridRule target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RulesRule target1 = (RulesRule) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (OnMapGridRule) target1;
    serialization.TryCustomCopy<OnMapGridRule>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref OnMapGridRule target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref RulesRule target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    OnMapGridRule target1 = (OnMapGridRule) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (RulesRule) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    OnMapGridRule target1 = (OnMapGridRule) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual OnMapGridRule RulesRule.Instantiate() => new OnMapGridRule();
}
