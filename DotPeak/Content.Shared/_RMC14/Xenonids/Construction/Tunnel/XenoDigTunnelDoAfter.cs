// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.Tunnel.XenoDigTunnelDoAfter
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction.Tunnel;

[NetSerializable]
[Serializable]
public sealed class XenoDigTunnelDoAfter : 
  SimpleDoAfterEvent,
  ISerializationGenerated<XenoDigTunnelDoAfter>,
  ISerializationGenerated
{
  public int PlasmaCost = 200;
  public string Prototype;

  public XenoDigTunnelDoAfter(EntProtoId prototype, int plasmaCost)
  {
    this.PlasmaCost = plasmaCost;
    this.Prototype = (string) prototype;
  }

  public XenoDigTunnelDoAfter()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoDigTunnelDoAfter target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoDigTunnelDoAfter) target1;
    serialization.TryCustomCopy<XenoDigTunnelDoAfter>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoDigTunnelDoAfter target,
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
    XenoDigTunnelDoAfter target1 = (XenoDigTunnelDoAfter) target;
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
    XenoDigTunnelDoAfter target1 = (XenoDigTunnelDoAfter) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual XenoDigTunnelDoAfter SimpleDoAfterEvent.Instantiate() => new XenoDigTunnelDoAfter();
}
