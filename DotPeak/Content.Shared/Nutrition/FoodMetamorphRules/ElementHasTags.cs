// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.FoodMetamorphRules.ElementHasTags
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
public sealed class ElementHasTags : 
  FoodMetamorphRule,
  ISerializationGenerated<ElementHasTags>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public int ElementNumber;
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
    FoodSequenceElementPrototype prototype;
    if (ingredients.Count < this.ElementNumber + 1 || !protoMan.TryIndex<FoodSequenceElementPrototype>(ingredients[this.ElementNumber].Proto, out prototype))
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
    ref ElementHasTags target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FoodMetamorphRule target1 = (FoodMetamorphRule) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ElementHasTags) target1;
    if (serialization.TryCustomCopy<ElementHasTags>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.ElementNumber, ref target2, hookCtx, false, context))
      target2 = this.ElementNumber;
    target.ElementNumber = target2;
    List<ProtoId<TagPrototype>> target3 = (List<ProtoId<TagPrototype>>) null;
    if (this.Tags == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<TagPrototype>>>(this.Tags, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<ProtoId<TagPrototype>>>(this.Tags, hookCtx, context);
    target.Tags = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.NeedAll, ref target4, hookCtx, false, context))
      target4 = this.NeedAll;
    target.NeedAll = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ElementHasTags target,
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
    ElementHasTags target1 = (ElementHasTags) target;
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
    ElementHasTags target1 = (ElementHasTags) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ElementHasTags FoodMetamorphRule.Instantiate() => new ElementHasTags();
}
