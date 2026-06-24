// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.Electrocute
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Electrocution;
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

public sealed class Electrocute : 
  EntityEffect,
  ISerializationGenerated<Electrocute>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public int ElectrocuteTime = 2;
  [DataField(null, false, 1, false, false, null)]
  public int ElectrocuteDamageScale = 5;
  [DataField(null, false, 1, false, false, null)]
  public bool Refresh = true;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-electrocute", ("chance", (object) this.Probability), ("time", (object) this.ElectrocuteTime));
  }

  public override bool ShouldLog => true;

  public override void Effect(EntityEffectBaseArgs args)
  {
    EntityEffectReagentArgs effectReagentArgs = args as EntityEffectReagentArgs;
    if ((object) effectReagentArgs != null)
    {
      effectReagentArgs.EntityManager.System<SharedElectrocutionSystem>().TryDoElectrocution(effectReagentArgs.TargetEntity, new EntityUid?(), Math.Max((effectReagentArgs.Quantity * this.ElectrocuteDamageScale).Int(), 1), TimeSpan.FromSeconds((long) this.ElectrocuteTime), this.Refresh, ignoreInsulation: true);
      if (effectReagentArgs.Reagent == null)
        return;
      effectReagentArgs.Source?.RemoveReagent(effectReagentArgs.Reagent.ID, effectReagentArgs.Quantity);
    }
    else
      args.EntityManager.System<SharedElectrocutionSystem>().TryDoElectrocution(args.TargetEntity, new EntityUid?(), Math.Max(this.ElectrocuteDamageScale, 1), TimeSpan.FromSeconds((long) this.ElectrocuteTime), this.Refresh, ignoreInsulation: true);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Electrocute target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Electrocute) target1;
    if (serialization.TryCustomCopy<Electrocute>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.ElectrocuteTime, ref target2, hookCtx, false, context))
      target2 = this.ElectrocuteTime;
    target.ElectrocuteTime = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.ElectrocuteDamageScale, ref target3, hookCtx, false, context))
      target3 = this.ElectrocuteDamageScale;
    target.ElectrocuteDamageScale = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Refresh, ref target4, hookCtx, false, context))
      target4 = this.Refresh;
    target.Refresh = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Electrocute target,
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
    Electrocute target1 = (Electrocute) target;
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
    Electrocute target1 = (Electrocute) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Electrocute EntityEffect.Instantiate() => new Electrocute();
}
