// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.Components.FoodSequenceElementComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Nutrition.Prototypes;
using Content.Shared.Tag;
using Robust.Shared.Analyzers;
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
namespace Content.Shared.Nutrition.Components;

[RegisterComponent]
[Access(new Type[] {typeof (SharedFoodSequenceSystem)})]
public sealed class FoodSequenceElementComponent : 
  Component,
  ISerializationGenerated<FoodSequenceElementComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public Dictionary<ProtoId<TagPrototype>, ProtoId<FoodSequenceElementPrototype>> Entries = new Dictionary<ProtoId<TagPrototype>, ProtoId<FoodSequenceElementPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public string Solution = "food";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FoodSequenceElementComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FoodSequenceElementComponent) target1;
    if (serialization.TryCustomCopy<FoodSequenceElementComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<ProtoId<TagPrototype>, ProtoId<FoodSequenceElementPrototype>> target2 = (Dictionary<ProtoId<TagPrototype>, ProtoId<FoodSequenceElementPrototype>>) null;
    if (this.Entries == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<TagPrototype>, ProtoId<FoodSequenceElementPrototype>>>(this.Entries, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<ProtoId<TagPrototype>, ProtoId<FoodSequenceElementPrototype>>>(this.Entries, hookCtx, context);
    target.Entries = target2;
    string target3 = (string) null;
    if (this.Solution == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Solution, ref target3, hookCtx, false, context))
      target3 = this.Solution;
    target.Solution = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FoodSequenceElementComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FoodSequenceElementComponent target1 = (FoodSequenceElementComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FoodSequenceElementComponent target1 = (FoodSequenceElementComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FoodSequenceElementComponent target1 = (FoodSequenceElementComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual FoodSequenceElementComponent Component.Instantiate()
  {
    return new FoodSequenceElementComponent();
  }
}
