// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.Tunnel.XenoDigTunnelActionEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction.Tunnel;

public sealed class XenoDigTunnelActionEvent : 
  InstantActionEvent,
  ISerializationGenerated<XenoDigTunnelActionEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId Prototype = (EntProtoId) "XenoTunnel";
  [DataField(null, false, 1, false, false, null)]
  public float DestroyWeedSourceDelay = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float CreateTunnelDelay = 4f;
  [DataField(null, false, 1, false, false, null)]
  public int PlasmaCost = 200;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoDigTunnelActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    InstantActionEvent target1 = (InstantActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoDigTunnelActionEvent) target1;
    if (serialization.TryCustomCopy<XenoDigTunnelActionEvent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Prototype, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Prototype, hookCtx, context);
    target.Prototype = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DestroyWeedSourceDelay, ref target3, hookCtx, false, context))
      target3 = this.DestroyWeedSourceDelay;
    target.DestroyWeedSourceDelay = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CreateTunnelDelay, ref target4, hookCtx, false, context))
      target4 = this.CreateTunnelDelay;
    target.CreateTunnelDelay = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.PlasmaCost, ref target5, hookCtx, false, context))
      target5 = this.PlasmaCost;
    target.PlasmaCost = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoDigTunnelActionEvent target,
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
    XenoDigTunnelActionEvent target1 = (XenoDigTunnelActionEvent) target;
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
    XenoDigTunnelActionEvent target1 = (XenoDigTunnelActionEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual XenoDigTunnelActionEvent InstantActionEvent.Instantiate()
  {
    return new XenoDigTunnelActionEvent();
  }
}
