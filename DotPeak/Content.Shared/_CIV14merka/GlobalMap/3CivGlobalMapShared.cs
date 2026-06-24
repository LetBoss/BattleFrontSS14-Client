// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.GlobalMap.QuickEnemyCivMarkerActionEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._CIV14merka.GlobalMap;

public sealed class QuickEnemyCivMarkerActionEvent : 
  WorldTargetActionEvent,
  ISerializationGenerated<QuickEnemyCivMarkerActionEvent>,
  ISerializationGenerated
{
  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref QuickEnemyCivMarkerActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    WorldTargetActionEvent target1 = (WorldTargetActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (QuickEnemyCivMarkerActionEvent) target1;
    serialization.TryCustomCopy<QuickEnemyCivMarkerActionEvent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref QuickEnemyCivMarkerActionEvent target,
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
    QuickEnemyCivMarkerActionEvent target1 = (QuickEnemyCivMarkerActionEvent) target;
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
    QuickEnemyCivMarkerActionEvent target1 = (QuickEnemyCivMarkerActionEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual QuickEnemyCivMarkerActionEvent WorldTargetActionEvent.Instantiate()
  {
    return new QuickEnemyCivMarkerActionEvent();
  }
}
