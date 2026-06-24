// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ensnaring.Components.RemoveEnsnareAlertEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Ensnaring.Components;

public sealed class RemoveEnsnareAlertEvent : 
  BaseAlertEvent,
  ISerializationGenerated<RemoveEnsnareAlertEvent>,
  ISerializationGenerated
{
  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RemoveEnsnareAlertEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BaseAlertEvent target1 = (BaseAlertEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RemoveEnsnareAlertEvent) target1;
    serialization.TryCustomCopy<RemoveEnsnareAlertEvent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RemoveEnsnareAlertEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref BaseAlertEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RemoveEnsnareAlertEvent target1 = (RemoveEnsnareAlertEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (BaseAlertEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RemoveEnsnareAlertEvent target1 = (RemoveEnsnareAlertEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual RemoveEnsnareAlertEvent BaseAlertEvent.Instantiate() => new RemoveEnsnareAlertEvent();
}
