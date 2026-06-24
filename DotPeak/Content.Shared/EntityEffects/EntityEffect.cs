// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.EntityEffect
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Database;
using Content.Shared.Localizations;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.EntityEffects;

[ImplicitDataDefinitionForInheritors]
public abstract class EntityEffect : ISerializationGenerated<EntityEffect>, ISerializationGenerated
{
  [DataField("conditions", false, 1, false, false, null)]
  public EntityEffectCondition[]? Conditions;
  [DataField("probability", false, 1, false, false, null)]
  public float Probability = 1f;

  private protected string _id => this.GetType().Name;

  public virtual string ReagentEffectFormat => "guidebook-reagent-effect-description";

  protected abstract string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys);

  public virtual LogImpact LogImpact { get; private set; } = LogImpact.Low;

  public virtual bool ShouldLog { get; private set; }

  public abstract void Effect(EntityEffectBaseArgs args);

  public string? GuidebookEffectDescription(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    string str = this.ReagentEffectGuidebookText(prototype, entSys);
    if (str == null)
      return (string) null;
    string reagentEffectFormat = this.ReagentEffectFormat;
    (string, object)[] valueTupleArray = new (string, object)[4];
    valueTupleArray[0] = ("effect", (object) str);
    valueTupleArray[1] = ("chance", (object) this.Probability);
    EntityEffectCondition[] conditions1 = this.Conditions;
    valueTupleArray[2] = ("conditionCount", (object) (conditions1 != null ? conditions1.Length : 0));
    EntityEffectCondition[] conditions2 = this.Conditions;
    valueTupleArray[3] = ("conditions", (object) ContentLocalizationManager.FormatList((conditions2 != null ? ((IEnumerable<EntityEffectCondition>) conditions2).Select<EntityEffectCondition, string>((Func<EntityEffectCondition, string>) (x => x.GuidebookExplanation(prototype))).ToList<string>() : (List<string>) null) ?? new List<string>()));
    return Loc.GetString(reagentEffectFormat, valueTupleArray);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref EntityEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<EntityEffect>(this, ref target, hookCtx, false, context))
      return;
    EntityEffectCondition[] target1 = (EntityEffectCondition[]) null;
    if (!serialization.TryCustomCopy<EntityEffectCondition[]>(this.Conditions, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<EntityEffectCondition[]>(this.Conditions, hookCtx, context);
    target.Conditions = target1;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Probability, ref target2, hookCtx, false, context))
      target2 = this.Probability;
    target.Probability = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref EntityEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual EntityEffect Instantiate() => throw new NotImplementedException();
}
