// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Intel.IntelSubmitDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Intel;

[NetSerializable]
[Serializable]
public sealed class IntelSubmitDoAfterEvent : 
  SimpleDoAfterEvent,
  ISerializationGenerated<IntelSubmitDoAfterEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public NetEntity Intel;
  [DataField(null, false, 1, false, false, null)]
  public int Amount;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IntelSubmitDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (IntelSubmitDoAfterEvent) target1;
    if (serialization.TryCustomCopy<IntelSubmitDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    NetEntity target2 = new NetEntity();
    if (!serialization.TryCustomCopy<NetEntity>(this.Intel, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<NetEntity>(this.Intel, hookCtx, context);
    target.Intel = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Amount, ref target3, hookCtx, false, context))
      target3 = this.Amount;
    target.Amount = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IntelSubmitDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref SimpleDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    IntelSubmitDoAfterEvent target1 = (IntelSubmitDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (SimpleDoAfterEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    IntelSubmitDoAfterEvent target1 = (IntelSubmitDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual IntelSubmitDoAfterEvent SimpleDoAfterEvent.Instantiate() => new IntelSubmitDoAfterEvent();
}
