// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.FoodMetamorphRules.LastElementHasTags
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.Prototypes;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Nutrition.FoodMetamorphRules;

[NetSerializable]
[Serializable]
public sealed class LastElementHasTags : 
  FoodMetamorphRule,
  ISerializationGenerated<LastElementHasTags>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public List<ProtoId<TagPrototype>> Tags = new List<ProtoId<TagPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public bool NeedAll = true;

  public override bool Check(
    IPrototypeManager protoMan,
    EntityManager entMan,
    EntityUid food,
    List<FoodSequenceVisualLayer> ingredients)
  {
    FoodSequenceVisualLayer ingredient = ingredients[ingredients.Count - 1];
    FoodSequenceElementPrototype prototype;
    if (!protoMan.TryIndex<FoodSequenceElementPrototype>(ingredient.Proto, out prototype))
      return false;
    foreach (ProtoId<TagPrototype> tag in this.Tags)
    {
      bool flag = prototype.Tags.Contains(tag);
      if (this.NeedAll && !flag)
        return false;
      if (!this.NeedAll & flag)
        return true;
    }
    return this.NeedAll;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref LastElementHasTags target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FoodMetamorphRule target1 = (FoodMetamorphRule) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (LastElementHasTags) target1;
    if (serialization.TryCustomCopy<LastElementHasTags>(this, ref target, hookCtx, false, context))
      return;
    List<ProtoId<TagPrototype>> target2 = (List<ProtoId<TagPrototype>>) null;
    if (this.Tags == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<TagPrototype>>>(this.Tags, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<ProtoId<TagPrototype>>>(this.Tags, hookCtx, context);
    target.Tags = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.NeedAll, ref target3, hookCtx, false, context))
      target3 = this.NeedAll;
    target.NeedAll = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref LastElementHasTags target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref FoodMetamorphRule target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    LastElementHasTags target1 = (LastElementHasTags) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (FoodMetamorphRule) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    LastElementHasTags target1 = (LastElementHasTags) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual LastElementHasTags FoodMetamorphRule.Instantiate() => new LastElementHasTags();
}
