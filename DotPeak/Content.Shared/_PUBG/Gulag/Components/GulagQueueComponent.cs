// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Gulag.Components.GulagQueueComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.Gulag.Components;

[RegisterComponent]
public sealed class GulagQueueComponent : 
  Component,
  ISerializationGenerated<GulagQueueComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public List<EntityUid> QueuedPlayers = new List<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<EntityUid, TimeSpan> QueueJoinTime = new Dictionary<EntityUid, TimeSpan>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GulagQueueComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GulagQueueComponent) target1;
    if (serialization.TryCustomCopy<GulagQueueComponent>(this, ref target, hookCtx, false, context))
      return;
    List<EntityUid> target2 = (List<EntityUid>) null;
    if (this.QueuedPlayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.QueuedPlayers, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<EntityUid>>(this.QueuedPlayers, hookCtx, context);
    target.QueuedPlayers = target2;
    Dictionary<EntityUid, TimeSpan> target3 = (Dictionary<EntityUid, TimeSpan>) null;
    if (this.QueueJoinTime == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntityUid, TimeSpan>>(this.QueueJoinTime, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<EntityUid, TimeSpan>>(this.QueueJoinTime, hookCtx, context);
    target.QueueJoinTime = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GulagQueueComponent target,
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
    GulagQueueComponent target1 = (GulagQueueComponent) target;
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
    GulagQueueComponent target1 = (GulagQueueComponent) target;
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
    GulagQueueComponent target1 = (GulagQueueComponent) target;
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
  virtual GulagQueueComponent Component.Instantiate() => new GulagQueueComponent();
}
