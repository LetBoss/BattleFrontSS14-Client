// Decompiled with JetBrains decompiler
// Type: Content.Shared.Random.RandomPlantMutation
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.EntityEffects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared.Random;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class RandomPlantMutation : 
  ISerializationGenerated<RandomPlantMutation>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float BaseOdds;
  [DataField(null, false, 1, false, false, null)]
  public string Name = "";
  [DataField(null, false, 1, false, false, null)]
  public LocId? Description;
  [DataField(null, false, 1, false, false, null)]
  public EntityEffect Effect;
  [DataField(null, false, 1, false, false, null)]
  public bool AppliesToProduce = true;
  [DataField(null, false, 1, false, false, null)]
  public bool AppliesToPlant = true;
  [DataField(null, false, 1, false, false, null)]
  public bool Persists = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RandomPlantMutation target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<RandomPlantMutation>(this, ref target, hookCtx, false, context))
      return;
    float target1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseOdds, ref target1, hookCtx, false, context))
      target1 = this.BaseOdds;
    target.BaseOdds = target1;
    string target2 = (string) null;
    if (this.Name == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Name, ref target2, hookCtx, false, context))
      target2 = this.Name;
    target.Name = target2;
    LocId? target3 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.Description, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<LocId?>(this.Description, hookCtx, context);
    target.Description = target3;
    EntityEffect target4 = (EntityEffect) null;
    if (this.Effect == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityEffect>(this.Effect, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<EntityEffect>(this.Effect, hookCtx, context);
    target.Effect = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.AppliesToProduce, ref target5, hookCtx, false, context))
      target5 = this.AppliesToProduce;
    target.AppliesToProduce = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.AppliesToPlant, ref target6, hookCtx, false, context))
      target6 = this.AppliesToPlant;
    target.AppliesToPlant = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.Persists, ref target7, hookCtx, false, context))
      target7 = this.Persists;
    target.Persists = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RandomPlantMutation target,
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
    RandomPlantMutation target1 = (RandomPlantMutation) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public RandomPlantMutation Instantiate() => new RandomPlantMutation();
}
