// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.Components.ExaminableHungerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
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
[NetworkedComponent]
[Access(new Type[] {typeof (ExaminableHungerSystem)})]
public sealed class ExaminableHungerComponent : 
  Component,
  ISerializationGenerated<ExaminableHungerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<HungerThreshold, LocId> Descriptions = new Dictionary<HungerThreshold, LocId>()
  {
    {
      HungerThreshold.Overfed,
      (LocId) "examinable-hunger-component-examine-overfed"
    },
    {
      HungerThreshold.Okay,
      (LocId) "examinable-hunger-component-examine-okay"
    },
    {
      HungerThreshold.Peckish,
      (LocId) "examinable-hunger-component-examine-peckish"
    },
    {
      HungerThreshold.Starving,
      (LocId) "examinable-hunger-component-examine-starving"
    },
    {
      HungerThreshold.Dead,
      (LocId) "examinable-hunger-component-examine-starving"
    }
  };
  public LocId NoHungerDescription = (LocId) "examinable-hunger-component-examine-none";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ExaminableHungerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ExaminableHungerComponent) target1;
    if (serialization.TryCustomCopy<ExaminableHungerComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<HungerThreshold, LocId> target2 = (Dictionary<HungerThreshold, LocId>) null;
    if (this.Descriptions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<HungerThreshold, LocId>>(this.Descriptions, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<HungerThreshold, LocId>>(this.Descriptions, hookCtx, context);
    target.Descriptions = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ExaminableHungerComponent target,
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
    ExaminableHungerComponent target1 = (ExaminableHungerComponent) target;
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
    ExaminableHungerComponent target1 = (ExaminableHungerComponent) target;
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
    ExaminableHungerComponent target1 = (ExaminableHungerComponent) target;
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
  virtual ExaminableHungerComponent Component.Instantiate() => new ExaminableHungerComponent();
}
