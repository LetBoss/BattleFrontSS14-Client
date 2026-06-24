// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.FoodMetamorphRules.IngredientsWithTags
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Destructible.Thresholds;
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
public sealed class IngredientsWithTags : 
  FoodMetamorphRule,
  ISerializationGenerated<IngredientsWithTags>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public List<ProtoId<TagPrototype>> Tags = new List<ProtoId<TagPrototype>>();
  [DataField(null, false, 1, true, false, null)]
  public MinMax Count = new MinMax();
  [DataField(null, false, 1, false, false, null)]
  public bool NeedAll = true;

  public override bool Check(
    IPrototypeManager protoMan,
    EntityManager entMan,
    EntityUid food,
    List<FoodSequenceVisualLayer> ingredients)
  {
    int num = 0;
    foreach (FoodSequenceVisualLayer ingredient in ingredients)
    {
      FoodSequenceElementPrototype prototype;
      if (protoMan.TryIndex<FoodSequenceElementPrototype>(ingredient.Proto, out prototype))
      {
        bool flag;
        if (this.NeedAll)
        {
          flag = true;
          foreach (ProtoId<TagPrototype> tag in this.Tags)
          {
            if (!prototype.Tags.Contains(tag))
            {
              flag = false;
              break;
            }
          }
        }
        else
        {
          flag = false;
          foreach (ProtoId<TagPrototype> tag in this.Tags)
          {
            if (prototype.Tags.Contains(tag))
            {
              flag = true;
              break;
            }
          }
        }
        if (flag)
          ++num;
      }
    }
    return num >= this.Count.Min && num <= this.Count.Max;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IngredientsWithTags target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FoodMetamorphRule target1 = (FoodMetamorphRule) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (IngredientsWithTags) target1;
    if (serialization.TryCustomCopy<IngredientsWithTags>(this, ref target, hookCtx, false, context))
      return;
    List<ProtoId<TagPrototype>> target2 = (List<ProtoId<TagPrototype>>) null;
    if (this.Tags == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<TagPrototype>>>(this.Tags, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<ProtoId<TagPrototype>>>(this.Tags, hookCtx, context);
    target.Tags = target2;
    MinMax target3 = new MinMax();
    if (!serialization.TryCustomCopy<MinMax>(this.Count, ref target3, hookCtx, false, context))
      serialization.CopyTo<MinMax>(this.Count, ref target3, hookCtx, context);
    target.Count = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.NeedAll, ref target4, hookCtx, false, context))
      target4 = this.NeedAll;
    target.NeedAll = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IngredientsWithTags target,
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
    IngredientsWithTags target1 = (IngredientsWithTags) target;
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
    IngredientsWithTags target1 = (IngredientsWithTags) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual IngredientsWithTags FoodMetamorphRule.Instantiate() => new IngredientsWithTags();
}
