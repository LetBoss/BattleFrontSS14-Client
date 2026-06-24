// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Destroy.XenoDestroyLeapDoafter
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
namespace Content.Shared._RMC14.Xenonids.Destroy;

[NetSerializable]
[Serializable]
public sealed class XenoDestroyLeapDoafter : 
  SimpleDoAfterEvent,
  ISerializationGenerated<XenoDestroyLeapDoafter>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public NetCoordinates TargetCoords;

  public XenoDestroyLeapDoafter(NetCoordinates coordinates) => this.TargetCoords = coordinates;

  public XenoDestroyLeapDoafter()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoDestroyLeapDoafter target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoDestroyLeapDoafter) target1;
    if (serialization.TryCustomCopy<XenoDestroyLeapDoafter>(this, ref target, hookCtx, false, context))
      return;
    NetCoordinates target2 = new NetCoordinates();
    if (!serialization.TryCustomCopy<NetCoordinates>(this.TargetCoords, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<NetCoordinates>(this.TargetCoords, hookCtx, context);
    target.TargetCoords = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoDestroyLeapDoafter target,
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
    XenoDestroyLeapDoafter target1 = (XenoDestroyLeapDoafter) target;
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
    XenoDestroyLeapDoafter target1 = (XenoDestroyLeapDoafter) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual XenoDestroyLeapDoafter SimpleDoAfterEvent.Instantiate() => new XenoDestroyLeapDoafter();
}
