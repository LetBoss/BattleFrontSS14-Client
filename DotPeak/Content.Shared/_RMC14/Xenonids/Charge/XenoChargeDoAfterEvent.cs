// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Charge.XenoChargeDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Charge;

[NetSerializable]
[Serializable]
public sealed class XenoChargeDoAfterEvent : 
  SimpleDoAfterEvent,
  ISerializationGenerated<XenoChargeDoAfterEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public NetCoordinates Coordinates;

  public XenoChargeDoAfterEvent(NetCoordinates coordinates) => this.Coordinates = coordinates;

  public XenoChargeDoAfterEvent()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoChargeDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoChargeDoAfterEvent) target1;
    if (serialization.TryCustomCopy<XenoChargeDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    NetCoordinates target2 = new NetCoordinates();
    if (!serialization.TryCustomCopy<NetCoordinates>(this.Coordinates, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<NetCoordinates>(this.Coordinates, hookCtx, context);
    target.Coordinates = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoChargeDoAfterEvent target,
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
    XenoChargeDoAfterEvent target1 = (XenoChargeDoAfterEvent) target;
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
    XenoChargeDoAfterEvent target1 = (XenoChargeDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual XenoChargeDoAfterEvent SimpleDoAfterEvent.Instantiate() => new XenoChargeDoAfterEvent();
}
