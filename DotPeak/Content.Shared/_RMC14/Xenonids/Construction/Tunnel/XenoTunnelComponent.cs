// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.Tunnel.XenoTunnelComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction.Tunnel;

[RegisterComponent]
public sealed class XenoTunnelComponent : 
  Component,
  ISerializationGenerated<XenoTunnelComponent>,
  ISerializationGenerated
{
  public const string ContainedMobsContainerId = "rmc_xeno_tunnel_mob_container";
  [DataField(null, false, 1, false, false, null)]
  public int MaxMobs = 3;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan SmallXenoEnterDelay = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan StandardXenoEnterDelay = TimeSpan.FromSeconds(4L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan LargeXenoEnterDelay = TimeSpan.FromSeconds(12L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan SmallXenoMoveDelay = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan StandardXenoMoveDelay = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan LargeXenoMoveDelay = TimeSpan.FromSeconds(6L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoTunnelComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoTunnelComponent) target1;
    if (serialization.TryCustomCopy<XenoTunnelComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxMobs, ref target2, hookCtx, false, context))
      target2 = this.MaxMobs;
    target.MaxMobs = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SmallXenoEnterDelay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.SmallXenoEnterDelay, hookCtx, context);
    target.SmallXenoEnterDelay = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StandardXenoEnterDelay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.StandardXenoEnterDelay, hookCtx, context);
    target.StandardXenoEnterDelay = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LargeXenoEnterDelay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.LargeXenoEnterDelay, hookCtx, context);
    target.LargeXenoEnterDelay = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SmallXenoMoveDelay, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.SmallXenoMoveDelay, hookCtx, context);
    target.SmallXenoMoveDelay = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StandardXenoMoveDelay, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.StandardXenoMoveDelay, hookCtx, context);
    target.StandardXenoMoveDelay = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LargeXenoMoveDelay, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.LargeXenoMoveDelay, hookCtx, context);
    target.LargeXenoMoveDelay = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoTunnelComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    XenoTunnelComponent target1 = (XenoTunnelComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    XenoTunnelComponent target1 = (XenoTunnelComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    XenoTunnelComponent target1 = (XenoTunnelComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual XenoTunnelComponent Component.Instantiate() => new XenoTunnelComponent();
}
