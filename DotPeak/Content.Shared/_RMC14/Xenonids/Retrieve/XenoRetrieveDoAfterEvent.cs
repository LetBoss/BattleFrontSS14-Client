// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Retrieve.XenoRetrieveDoAfterEvent
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
namespace Content.Shared._RMC14.Xenonids.Retrieve;

[NetSerializable]
[Serializable]
public sealed class XenoRetrieveDoAfterEvent : 
  SimpleDoAfterEvent,
  ISerializationGenerated<XenoRetrieveDoAfterEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public NetEntity Action;

  public XenoRetrieveDoAfterEvent(NetEntity action) => this.Action = action;

  public XenoRetrieveDoAfterEvent()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoRetrieveDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoRetrieveDoAfterEvent) target1;
    if (serialization.TryCustomCopy<XenoRetrieveDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    NetEntity target2 = new NetEntity();
    if (!serialization.TryCustomCopy<NetEntity>(this.Action, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<NetEntity>(this.Action, hookCtx, context);
    target.Action = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoRetrieveDoAfterEvent target,
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
    XenoRetrieveDoAfterEvent target1 = (XenoRetrieveDoAfterEvent) target;
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
    XenoRetrieveDoAfterEvent target1 = (XenoRetrieveDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual XenoRetrieveDoAfterEvent SimpleDoAfterEvent.Instantiate()
  {
    return new XenoRetrieveDoAfterEvent();
  }
}
