// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.PlantMetabolism.PlantCryoxadone
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects.PlantMetabolism;

public sealed class PlantCryoxadone : 
  EventEntityEffect<PlantCryoxadone>,
  ISerializationGenerated<PlantCryoxadone>,
  ISerializationGenerated
{
  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-plant-cryoxadone", ("chance", (object) this.Probability));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PlantCryoxadone target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<PlantCryoxadone> target1 = (EventEntityEffect<PlantCryoxadone>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PlantCryoxadone) target1;
    serialization.TryCustomCopy<PlantCryoxadone>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PlantCryoxadone target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<PlantCryoxadone> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PlantCryoxadone target1 = (PlantCryoxadone) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<PlantCryoxadone>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PlantCryoxadone target1 = (PlantCryoxadone) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual PlantCryoxadone EventEntityEffect<PlantCryoxadone>.Instantiate() => new PlantCryoxadone();
}
