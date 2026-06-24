// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Tail_Lash.XenoTailLashActionEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Tail_Lash;

public sealed class XenoTailLashActionEvent : 
  WorldTargetActionEvent,
  ISerializationGenerated<XenoTailLashActionEvent>,
  ISerializationGenerated
{
  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoTailLashActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    WorldTargetActionEvent target1 = (WorldTargetActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoTailLashActionEvent) target1;
    serialization.TryCustomCopy<XenoTailLashActionEvent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoTailLashActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref WorldTargetActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    XenoTailLashActionEvent target1 = (XenoTailLashActionEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (WorldTargetActionEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    XenoTailLashActionEvent target1 = (XenoTailLashActionEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual XenoTailLashActionEvent WorldTargetActionEvent.Instantiate()
  {
    return new XenoTailLashActionEvent();
  }
}
