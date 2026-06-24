// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.PowerLoader.Events.DropshipAttachDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.PowerLoader.Events;

[NetSerializable]
[Serializable]
public sealed class DropshipAttachDoAfterEvent : 
  DropshipDoAfterEvent,
  ISerializationGenerated<DropshipAttachDoAfterEvent>,
  ISerializationGenerated
{
  public DropshipAttachDoAfterEvent(NetEntity container, NetEntity contained, string slot)
    : base(container, contained, slot)
  {
  }

  public DropshipAttachDoAfterEvent()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DropshipAttachDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DropshipDoAfterEvent target1 = (DropshipDoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DropshipAttachDoAfterEvent) target1;
    serialization.TryCustomCopy<DropshipAttachDoAfterEvent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DropshipAttachDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref DropshipDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DropshipAttachDoAfterEvent target1 = (DropshipAttachDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (DropshipDoAfterEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DropshipAttachDoAfterEvent target1 = (DropshipAttachDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual DropshipAttachDoAfterEvent DropshipDoAfterEvent.Instantiate()
  {
    return new DropshipAttachDoAfterEvent();
  }
}
