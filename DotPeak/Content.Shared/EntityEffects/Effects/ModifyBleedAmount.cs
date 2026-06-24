// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.ModifyBleedAmount
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

public sealed class ModifyBleedAmount : 
  EventEntityEffect<ModifyBleedAmount>,
  ISerializationGenerated<ModifyBleedAmount>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool Scaled;
  [DataField(null, false, 1, false, false, null)]
  public float Amount = -1f;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-modify-bleed-amount", ("chance", (object) this.Probability), ("deltasign", (object) MathF.Sign(this.Amount)));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ModifyBleedAmount target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<ModifyBleedAmount> target1 = (EventEntityEffect<ModifyBleedAmount>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ModifyBleedAmount) target1;
    if (serialization.TryCustomCopy<ModifyBleedAmount>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Scaled, ref target2, hookCtx, false, context))
      target2 = this.Scaled;
    target.Scaled = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Amount, ref target3, hookCtx, false, context))
      target3 = this.Amount;
    target.Amount = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ModifyBleedAmount target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<ModifyBleedAmount> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ModifyBleedAmount target1 = (ModifyBleedAmount) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<ModifyBleedAmount>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ModifyBleedAmount target1 = (ModifyBleedAmount) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ModifyBleedAmount EventEntityEffect<ModifyBleedAmount>.Instantiate()
  {
    return new ModifyBleedAmount();
  }
}
