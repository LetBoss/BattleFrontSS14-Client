// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Fruit.Events.XenoFruitPlantActionEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Fruit.Events;

public sealed class XenoFruitPlantActionEvent : 
  InstantActionEvent,
  ISerializationGenerated<XenoFruitPlantActionEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool CheckWeeds = true;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 PlasmaCost = (FixedPoint2) 100;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 HealthCost = (FixedPoint2) 50;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoFruitPlantActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    InstantActionEvent target1 = (InstantActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoFruitPlantActionEvent) target1;
    if (serialization.TryCustomCopy<XenoFruitPlantActionEvent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.CheckWeeds, ref target2, hookCtx, false, context))
      target2 = this.CheckWeeds;
    target.CheckWeeds = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaCost, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.PlasmaCost, hookCtx, context);
    target.PlasmaCost = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.HealthCost, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.HealthCost, hookCtx, context);
    target.HealthCost = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoFruitPlantActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref InstantActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    XenoFruitPlantActionEvent target1 = (XenoFruitPlantActionEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (InstantActionEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    XenoFruitPlantActionEvent target1 = (XenoFruitPlantActionEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual XenoFruitPlantActionEvent InstantActionEvent.Instantiate()
  {
    return new XenoFruitPlantActionEvent();
  }
}
