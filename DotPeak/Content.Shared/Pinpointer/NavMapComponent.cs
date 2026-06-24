// Decompiled with JetBrains decompiler
// Type: Content.Shared.Pinpointer.NavMapComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Pinpointer;

[RegisterComponent]
[NetworkedComponent]
public sealed class NavMapComponent : 
  Component,
  ISerializationGenerated<NavMapComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public Dictionary<Vector2i, NavMapChunk> Chunks = new Dictionary<Vector2i, NavMapChunk>();
  [Robust.Shared.ViewVariables.ViewVariables]
  public Dictionary<NetEntity, SharedNavMapSystem.NavMapBeacon> Beacons = new Dictionary<NetEntity, SharedNavMapSystem.NavMapBeacon>();
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public Dictionary<NetEntity, SharedNavMapSystem.NavMapRegionProperties> RegionProperties = new Dictionary<NetEntity, SharedNavMapSystem.NavMapRegionProperties>();
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public Dictionary<NetEntity, NavMapRegionOverlay> RegionOverlays = new Dictionary<NetEntity, NavMapRegionOverlay>();
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public Queue<NetEntity> QueuedRegionsToFlood = new Queue<NetEntity>();
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public Dictionary<Vector2i, HashSet<NetEntity>> ChunkToRegionOwnerTable = new Dictionary<Vector2i, HashSet<NetEntity>>();
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public Dictionary<NetEntity, HashSet<Vector2i>> RegionOwnerToChunkTable = new Dictionary<NetEntity, HashSet<Vector2i>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NavMapComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (NavMapComponent) target1;
    serialization.TryCustomCopy<NavMapComponent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NavMapComponent target,
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
    NavMapComponent target1 = (NavMapComponent) target;
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
    NavMapComponent target1 = (NavMapComponent) target;
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
    NavMapComponent target1 = (NavMapComponent) target;
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
  virtual NavMapComponent Component.Instantiate() => new NavMapComponent();
}
