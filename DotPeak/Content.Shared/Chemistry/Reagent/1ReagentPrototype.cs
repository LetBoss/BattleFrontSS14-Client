// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Reagent.ReagentEffectsEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

#nullable enable
namespace Content.Shared.Chemistry.Reagent;

[DataDefinition]
public sealed class ReagentEffectsEntry : 
  ISerializationGenerated<ReagentEffectsEntry>,
  ISerializationGenerated
{
  [JsonPropertyName("rate")]
  [DataField("metabolismRate", false, 1, false, false, null)]
  public FixedPoint2 MetabolismRate = FixedPoint2.New(0.5f);
  [JsonPropertyName("effects")]
  [DataField("effects", false, 1, true, false, null)]
  public EntityEffect[] Effects;

  public ReagentEffectsGuideEntry MakeGuideEntry(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return new ReagentEffectsGuideEntry(this.MetabolismRate, ((IEnumerable<EntityEffect>) this.Effects).Select<EntityEffect, string>((Func<EntityEffect, string>) (x => x.GuidebookEffectDescription(prototype, entSys))).Where<string>((Func<string, bool>) (x => x != null)).Select<string, string>((Func<string, string>) (x => x)).ToArray<string>());
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ReagentEffectsEntry target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ReagentEffectsEntry>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 fixedPoint2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MetabolismRate, ref fixedPoint2, hookCtx, false, context))
      fixedPoint2 = serialization.CreateCopy<FixedPoint2>(this.MetabolismRate, hookCtx, context, false);
    target.MetabolismRate = fixedPoint2;
    EntityEffect[] entityEffectArray = (EntityEffect[]) null;
    if (this.Effects == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityEffect[]>(this.Effects, ref entityEffectArray, hookCtx, true, context))
      entityEffectArray = serialization.CreateCopy<EntityEffect[]>(this.Effects, hookCtx, context, false);
    target.Effects = entityEffectArray;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ReagentEffectsEntry target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ReagentEffectsEntry target1 = (ReagentEffectsEntry) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public ReagentEffectsEntry Instantiate() => new ReagentEffectsEntry();
}
