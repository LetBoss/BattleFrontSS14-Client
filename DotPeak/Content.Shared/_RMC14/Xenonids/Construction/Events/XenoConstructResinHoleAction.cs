// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.Events.XenoConstructResinHoleActionEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction.Events;

public sealed class XenoConstructResinHoleActionEvent : 
  InstantActionEvent,
  ISerializationGenerated<XenoConstructResinHoleActionEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 PlasmaCost = (FixedPoint2) 200;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId Prototype = (EntProtoId) "XenoResinHole";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoConstructResinHoleActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    InstantActionEvent target1 = (InstantActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoConstructResinHoleActionEvent) target1;
    if (serialization.TryCustomCopy<XenoConstructResinHoleActionEvent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaCost, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.PlasmaCost, hookCtx, context);
    target.PlasmaCost = target2;
    EntProtoId target3 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Prototype, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId>(this.Prototype, hookCtx, context);
    target.Prototype = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoConstructResinHoleActionEvent target,
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
    XenoConstructResinHoleActionEvent target1 = (XenoConstructResinHoleActionEvent) target;
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
    XenoConstructResinHoleActionEvent target1 = (XenoConstructResinHoleActionEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual XenoConstructResinHoleActionEvent InstantActionEvent.Instantiate()
  {
    return new XenoConstructResinHoleActionEvent();
  }
}
