// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.Ignite
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Database;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

public sealed class Ignite : 
  EventEntityEffect<Ignite>,
  ISerializationGenerated<Ignite>,
  ISerializationGenerated
{
  public override bool ShouldLog => true;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-ignite", ("chance", (object) this.Probability));
  }

  public override LogImpact LogImpact => LogImpact.Medium;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Ignite target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<Ignite> target1 = (EventEntityEffect<Ignite>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Ignite) target1;
    serialization.TryCustomCopy<Ignite>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Ignite target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<Ignite> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Ignite target1 = (Ignite) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<Ignite>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Ignite target1 = (Ignite) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Ignite EventEntityEffect<Ignite>.Instantiate() => new Ignite();
}
