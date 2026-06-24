// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Tracker.SquadLeader.SquadLeaderTrackerClickedEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Tracker.SquadLeader;

public sealed class SquadLeaderTrackerClickedEvent : 
  BaseAlertEvent,
  ISerializationGenerated<SquadLeaderTrackerClickedEvent>,
  ISerializationGenerated
{
  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SquadLeaderTrackerClickedEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BaseAlertEvent target1 = (BaseAlertEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SquadLeaderTrackerClickedEvent) target1;
    serialization.TryCustomCopy<SquadLeaderTrackerClickedEvent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SquadLeaderTrackerClickedEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref BaseAlertEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SquadLeaderTrackerClickedEvent target1 = (SquadLeaderTrackerClickedEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (BaseAlertEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SquadLeaderTrackerClickedEvent target1 = (SquadLeaderTrackerClickedEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual SquadLeaderTrackerClickedEvent BaseAlertEvent.Instantiate()
  {
    return new SquadLeaderTrackerClickedEvent();
  }
}
