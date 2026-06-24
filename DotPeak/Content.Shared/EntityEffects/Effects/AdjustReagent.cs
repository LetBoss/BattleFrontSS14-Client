// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.AdjustReagent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Body.Prototypes;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

public sealed class AdjustReagent : 
  EntityEffect,
  ISerializationGenerated<AdjustReagent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, typeof (PrototypeIdSerializer<ReagentPrototype>))]
  public string? Reagent;
  [DataField(null, false, 1, false, false, typeof (PrototypeIdSerializer<MetabolismGroupPrototype>))]
  public string? Group;
  [DataField(null, false, 1, true, false, null)]
  public FixedPoint2 Amount;

  public override void Effect(EntityEffectBaseArgs args)
  {
    EntityEffectReagentArgs effectReagentArgs = args as EntityEffectReagentArgs;
    if ((object) effectReagentArgs == null)
      throw new NotImplementedException();
    if (effectReagentArgs.Source == null)
      return;
    FixedPoint2 quantity = this.Amount * effectReagentArgs.Scale;
    if (this.Reagent != null)
    {
      if (quantity < 0 && effectReagentArgs.Source.ContainsPrototype(this.Reagent))
        effectReagentArgs.Source.RemoveReagent(this.Reagent, -quantity);
      if (!(quantity > 0))
        return;
      effectReagentArgs.Source.AddReagent(this.Reagent, quantity);
    }
    else
    {
      if (this.Group == null)
        return;
      RMCReagentSystem rmcReagentSystem = IoCManager.Resolve<IEntityManager>().System<RMCReagentSystem>();
      foreach (ReagentQuantity reagentQuantity in effectReagentArgs.Source.Contents.ToArray())
      {
        Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent = rmcReagentSystem.Index((ProtoId<ReagentPrototype>) reagentQuantity.Reagent.Prototype);
        if (reagent.Metabolisms != null && reagent.Metabolisms.ContainsKey((ProtoId<MetabolismGroupPrototype>) this.Group))
        {
          if (quantity < 0)
            effectReagentArgs.Source.RemoveReagent(reagentQuantity.Reagent, quantity);
          if (quantity > 0)
            effectReagentArgs.Source.AddReagent(reagentQuantity.Reagent, quantity);
        }
      }
    }
  }

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent;
    if (this.Reagent != null && entSys.GetEntitySystem<RMCReagentSystem>().TryIndex((ProtoId<ReagentPrototype>) this.Reagent, out reagent))
      return Loc.GetString("reagent-effect-guidebook-adjust-reagent-reagent", ("chance", (object) this.Probability), ("deltasign", (object) MathF.Sign(this.Amount.Float())), ("reagent", (object) reagent.LocalizedName), ("amount", (object) MathF.Abs(this.Amount.Float())));
    MetabolismGroupPrototype prototype1;
    if (this.Group == null || !prototype.TryIndex<MetabolismGroupPrototype>(this.Group, out prototype1))
      throw new NotImplementedException();
    return Loc.GetString("reagent-effect-guidebook-adjust-reagent-group", ("chance", (object) this.Probability), ("deltasign", (object) MathF.Sign(this.Amount.Float())), ("group", (object) prototype1.LocalizedName), ("amount", (object) MathF.Abs(this.Amount.Float())));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AdjustReagent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AdjustReagent) target1;
    if (serialization.TryCustomCopy<AdjustReagent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Reagent, ref target2, hookCtx, false, context))
      target2 = this.Reagent;
    target.Reagent = target2;
    string target3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Group, ref target3, hookCtx, false, context))
      target3 = this.Group;
    target.Group = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Amount, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.Amount, hookCtx, context);
    target.Amount = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AdjustReagent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EntityEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AdjustReagent target1 = (AdjustReagent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EntityEffect) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AdjustReagent target1 = (AdjustReagent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual AdjustReagent EntityEffect.Instantiate() => new AdjustReagent();
}
