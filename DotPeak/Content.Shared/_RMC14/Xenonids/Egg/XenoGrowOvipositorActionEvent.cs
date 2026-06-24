// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Egg.XenoGrowOvipositorActionEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Egg;

public sealed class XenoGrowOvipositorActionEvent : 
  InstantActionEvent,
  ISerializationGenerated<XenoGrowOvipositorActionEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public int AttachPlasmaCost = 700;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan AttachDoAfter = TimeSpan.FromSeconds(20L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan DetachDoAfter = TimeSpan.FromSeconds(5L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoGrowOvipositorActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    InstantActionEvent target1 = (InstantActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoGrowOvipositorActionEvent) target1;
    if (serialization.TryCustomCopy<XenoGrowOvipositorActionEvent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.AttachPlasmaCost, ref target2, hookCtx, false, context))
      target2 = this.AttachPlasmaCost;
    target.AttachPlasmaCost = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AttachDoAfter, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.AttachDoAfter, hookCtx, context);
    target.AttachDoAfter = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DetachDoAfter, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.DetachDoAfter, hookCtx, context);
    target.DetachDoAfter = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoGrowOvipositorActionEvent target,
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
    XenoGrowOvipositorActionEvent target1 = (XenoGrowOvipositorActionEvent) target;
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
    XenoGrowOvipositorActionEvent target1 = (XenoGrowOvipositorActionEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual XenoGrowOvipositorActionEvent InstantActionEvent.Instantiate()
  {
    return new XenoGrowOvipositorActionEvent();
  }
}
