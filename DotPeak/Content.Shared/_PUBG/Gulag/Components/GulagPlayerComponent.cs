// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Gulag.Components.GulagPlayerComponent
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
namespace Content.Shared._PUBG.Gulag.Components;

[RegisterComponent]
public sealed class GulagPlayerComponent : 
  Component,
  ISerializationGenerated<GulagPlayerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public GulagPlayerState State;
  [DataField(null, false, 1, false, false, null)]
  public int? FrozenPlacement;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? GhostProtectionExpiry;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GulagPlayerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GulagPlayerComponent) target1;
    if (serialization.TryCustomCopy<GulagPlayerComponent>(this, ref target, hookCtx, false, context))
      return;
    GulagPlayerState target2 = GulagPlayerState.Queued;
    if (!serialization.TryCustomCopy<GulagPlayerState>(this.State, ref target2, hookCtx, false, context))
      target2 = this.State;
    target.State = target2;
    int? target3 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.FrozenPlacement, ref target3, hookCtx, false, context))
      target3 = this.FrozenPlacement;
    target.FrozenPlacement = target3;
    TimeSpan? target4 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.GhostProtectionExpiry, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan?>(this.GhostProtectionExpiry, hookCtx, context);
    target.GhostProtectionExpiry = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GulagPlayerComponent target,
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
    GulagPlayerComponent target1 = (GulagPlayerComponent) target;
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
    GulagPlayerComponent target1 = (GulagPlayerComponent) target;
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
    GulagPlayerComponent target1 = (GulagPlayerComponent) target;
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
  virtual GulagPlayerComponent Component.Instantiate() => new GulagPlayerComponent();
}
