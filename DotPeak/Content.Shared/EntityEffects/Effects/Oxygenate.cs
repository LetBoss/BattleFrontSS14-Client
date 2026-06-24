// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.Oxygenate
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

public sealed class Oxygenate : 
  EventEntityEffect<Oxygenate>,
  ISerializationGenerated<Oxygenate>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float Factor = 1f;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return (string) null;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Oxygenate target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<Oxygenate> target1 = (EventEntityEffect<Oxygenate>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Oxygenate) target1;
    if (serialization.TryCustomCopy<Oxygenate>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Factor, ref target2, hookCtx, false, context))
      target2 = this.Factor;
    target.Factor = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Oxygenate target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<Oxygenate> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Oxygenate target1 = (Oxygenate) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<Oxygenate>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Oxygenate target1 = (Oxygenate) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Oxygenate EventEntityEffect<Oxygenate>.Instantiate() => new Oxygenate();
}
