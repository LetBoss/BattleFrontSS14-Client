// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.Tunnel.XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction.Tunnel;

[NetSerializable]
[Serializable]
public sealed class XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent : 
  SimpleDoAfterEvent,
  ISerializationGenerated<XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId Prototype = (EntProtoId) "XenoTunnel";
  [DataField(null, false, 1, false, false, null)]
  public float CreateTunnelDelay = 4f;
  [DataField(null, false, 1, false, false, null)]
  public int PlasmaCost = 200;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent) target1;
    if (serialization.TryCustomCopy<XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Prototype, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Prototype, hookCtx, context);
    target.Prototype = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CreateTunnelDelay, ref target3, hookCtx, false, context))
      target3 = this.CreateTunnelDelay;
    target.CreateTunnelDelay = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.PlasmaCost, ref target4, hookCtx, false, context))
      target4 = this.PlasmaCost;
    target.PlasmaCost = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent target,
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
    XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent target1 = (XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent) target;
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
    XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent target1 = (XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent SimpleDoAfterEvent.Instantiate()
  {
    return new XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent();
  }
}
