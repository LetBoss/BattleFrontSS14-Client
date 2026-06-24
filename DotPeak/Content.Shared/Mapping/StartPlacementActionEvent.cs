// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mapping.StartPlacementActionEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Maps;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Mapping;

public sealed class StartPlacementActionEvent : 
  InstantActionEvent,
  ISerializationGenerated<StartPlacementActionEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? EntityType;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ContentTileDefinition>? TileId;
  [DataField(null, false, 1, false, false, null)]
  public string? PlacementOption;
  [DataField(null, false, 1, false, false, null)]
  public bool Eraser;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StartPlacementActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    InstantActionEvent target1 = (InstantActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StartPlacementActionEvent) target1;
    if (serialization.TryCustomCopy<StartPlacementActionEvent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId? target2 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.EntityType, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId?>(this.EntityType, hookCtx, context);
    target.EntityType = target2;
    ProtoId<ContentTileDefinition>? target3 = new ProtoId<ContentTileDefinition>?();
    if (!serialization.TryCustomCopy<ProtoId<ContentTileDefinition>?>(this.TileId, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<ContentTileDefinition>?>(this.TileId, hookCtx, context);
    target.TileId = target3;
    string target4 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.PlacementOption, ref target4, hookCtx, false, context))
      target4 = this.PlacementOption;
    target.PlacementOption = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Eraser, ref target5, hookCtx, false, context))
      target5 = this.Eraser;
    target.Eraser = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StartPlacementActionEvent target,
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
    StartPlacementActionEvent target1 = (StartPlacementActionEvent) target;
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
    StartPlacementActionEvent target1 = (StartPlacementActionEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual StartPlacementActionEvent InstantActionEvent.Instantiate()
  {
    return new StartPlacementActionEvent();
  }
}
