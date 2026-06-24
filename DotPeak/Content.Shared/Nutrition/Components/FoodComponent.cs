// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.Components.FoodComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Nutrition.Components;

[RegisterComponent]
[Access(new Type[] {typeof (FoodSystem), typeof (FoodSequenceSystem)})]
public sealed class FoodComponent : 
  Component,
  ISerializationGenerated<FoodComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string Solution = "food";
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier UseSound = (SoundSpecifier) new SoundCollectionSpecifier("eating");
  [DataField(null, false, 1, false, false, null)]
  public List<EntProtoId> Trash = new List<EntProtoId>();
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2? TransferAmount = new FixedPoint2?(FixedPoint2.New(5));
  [DataField(null, false, 1, false, false, null)]
  public UtensilType Utensil = UtensilType.Fork;
  [DataField(null, false, 1, false, false, null)]
  public bool UtensilRequired;
  [DataField(null, false, 1, false, false, null)]
  public bool RequiresSpecialDigestion;
  [DataField(null, false, 1, false, false, null)]
  public int RequiredStomachs = 1;
  [DataField(null, false, 1, false, false, null)]
  public LocId EatMessage = (LocId) "food-nom";
  [DataField(null, false, 1, false, false, null)]
  public float Delay = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  public float ForceFeedDelay = 3f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool RequireDead = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FoodComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FoodComponent) target1;
    if (serialization.TryCustomCopy<FoodComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Solution == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Solution, ref target2, hookCtx, false, context))
      target2 = this.Solution;
    target.Solution = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (this.UseSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.UseSound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.UseSound, hookCtx, context);
    target.UseSound = target3;
    List<EntProtoId> target4 = (List<EntProtoId>) null;
    if (this.Trash == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.Trash, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<EntProtoId>>(this.Trash, hookCtx, context);
    target.Trash = target4;
    FixedPoint2? target5 = new FixedPoint2?();
    if (!serialization.TryCustomCopy<FixedPoint2?>(this.TransferAmount, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<FixedPoint2?>(this.TransferAmount, hookCtx, context);
    target.TransferAmount = target5;
    UtensilType target6 = UtensilType.None;
    if (!serialization.TryCustomCopy<UtensilType>(this.Utensil, ref target6, hookCtx, false, context))
      target6 = this.Utensil;
    target.Utensil = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.UtensilRequired, ref target7, hookCtx, false, context))
      target7 = this.UtensilRequired;
    target.UtensilRequired = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequiresSpecialDigestion, ref target8, hookCtx, false, context))
      target8 = this.RequiresSpecialDigestion;
    target.RequiresSpecialDigestion = target8;
    int target9 = 0;
    if (!serialization.TryCustomCopy<int>(this.RequiredStomachs, ref target9, hookCtx, false, context))
      target9 = this.RequiredStomachs;
    target.RequiredStomachs = target9;
    LocId target10 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.EatMessage, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<LocId>(this.EatMessage, hookCtx, context);
    target.EatMessage = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Delay, ref target11, hookCtx, false, context))
      target11 = this.Delay;
    target.Delay = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ForceFeedDelay, ref target12, hookCtx, false, context))
      target12 = this.ForceFeedDelay;
    target.ForceFeedDelay = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequireDead, ref target13, hookCtx, false, context))
      target13 = this.RequireDead;
    target.RequireDead = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FoodComponent target,
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
    FoodComponent target1 = (FoodComponent) target;
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
    FoodComponent target1 = (FoodComponent) target;
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
    FoodComponent target1 = (FoodComponent) target;
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
  virtual FoodComponent Component.Instantiate() => new FoodComponent();
}
