// Decompiled with JetBrains decompiler
// Type: Content.Shared.RatKing.RatKingOrderActionEvent
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
namespace Content.Shared.RatKing;

public sealed class RatKingOrderActionEvent : 
  InstantActionEvent,
  ISerializationGenerated<RatKingOrderActionEvent>,
  ISerializationGenerated
{
  [DataField("type", false, 1, false, false, null)]
  public RatKingOrderType Type;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RatKingOrderActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    InstantActionEvent target1 = (InstantActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RatKingOrderActionEvent) target1;
    if (serialization.TryCustomCopy<RatKingOrderActionEvent>(this, ref target, hookCtx, false, context))
      return;
    RatKingOrderType target2 = RatKingOrderType.Stay;
    if (!serialization.TryCustomCopy<RatKingOrderType>(this.Type, ref target2, hookCtx, false, context))
      target2 = this.Type;
    target.Type = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RatKingOrderActionEvent target,
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
    RatKingOrderActionEvent target1 = (RatKingOrderActionEvent) target;
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
    RatKingOrderActionEvent target1 = (RatKingOrderActionEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual RatKingOrderActionEvent InstantActionEvent.Instantiate() => new RatKingOrderActionEvent();
}
