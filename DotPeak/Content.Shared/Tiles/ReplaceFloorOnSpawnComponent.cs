// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tiles.ReplaceFloorOnSpawnComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Maps;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Tiles;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (ReplaceFloorOnSpawnSystem)})]
public sealed class ReplaceFloorOnSpawnComponent : 
  Component,
  ISerializationGenerated<ReplaceFloorOnSpawnComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<ContentTileDefinition>>? ReplaceableTiles = new List<ProtoId<ContentTileDefinition>>();
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<ContentTileDefinition>> ReplacementTiles = new List<ProtoId<ContentTileDefinition>>();
  [DataField(null, false, 1, false, false, null)]
  public bool ReplaceSpace = true;
  [DataField(null, false, 1, false, false, null)]
  public List<Vector2i> Offsets = new List<Vector2i>()
  {
    Vector2i.Up,
    Vector2i.Down,
    Vector2i.Left,
    Vector2i.Right,
    Vector2i.Zero
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ReplaceFloorOnSpawnComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ReplaceFloorOnSpawnComponent) target1;
    if (serialization.TryCustomCopy<ReplaceFloorOnSpawnComponent>(this, ref target, hookCtx, false, context))
      return;
    List<ProtoId<ContentTileDefinition>> target2 = (List<ProtoId<ContentTileDefinition>>) null;
    if (!serialization.TryCustomCopy<List<ProtoId<ContentTileDefinition>>>(this.ReplaceableTiles, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<ProtoId<ContentTileDefinition>>>(this.ReplaceableTiles, hookCtx, context);
    target.ReplaceableTiles = target2;
    List<ProtoId<ContentTileDefinition>> target3 = (List<ProtoId<ContentTileDefinition>>) null;
    if (this.ReplacementTiles == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<ContentTileDefinition>>>(this.ReplacementTiles, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<ProtoId<ContentTileDefinition>>>(this.ReplacementTiles, hookCtx, context);
    target.ReplacementTiles = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.ReplaceSpace, ref target4, hookCtx, false, context))
      target4 = this.ReplaceSpace;
    target.ReplaceSpace = target4;
    List<Vector2i> target5 = (List<Vector2i>) null;
    if (this.Offsets == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<Vector2i>>(this.Offsets, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<List<Vector2i>>(this.Offsets, hookCtx, context);
    target.Offsets = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ReplaceFloorOnSpawnComponent target,
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
    ReplaceFloorOnSpawnComponent target1 = (ReplaceFloorOnSpawnComponent) target;
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
    ReplaceFloorOnSpawnComponent target1 = (ReplaceFloorOnSpawnComponent) target;
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
    ReplaceFloorOnSpawnComponent target1 = (ReplaceFloorOnSpawnComponent) target;
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
  virtual ReplaceFloorOnSpawnComponent Component.Instantiate()
  {
    return new ReplaceFloorOnSpawnComponent();
  }
}
