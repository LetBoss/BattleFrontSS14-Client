// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ghost.Roles.Components.GhostRoleMobSpawnerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Ghost.Roles.Components;

[RegisterComponent]
[EntityCategory(new string[] {"Spawner"})]
public sealed class GhostRoleMobSpawnerComponent : 
  Component,
  ISerializationGenerated<GhostRoleMobSpawnerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool DeleteOnSpawn = true;
  [DataField(null, false, 1, false, false, null)]
  public int AvailableTakeovers = 1;
  [Robust.Shared.ViewVariables.ViewVariables]
  public int CurrentTakeovers;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? Prototype;
  [DataField(null, false, 1, false, false, null)]
  public List<string> SelectablePrototypes = new List<string>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GhostRoleMobSpawnerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GhostRoleMobSpawnerComponent) target1;
    if (serialization.TryCustomCopy<GhostRoleMobSpawnerComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.DeleteOnSpawn, ref target2, hookCtx, false, context))
      target2 = this.DeleteOnSpawn;
    target.DeleteOnSpawn = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.AvailableTakeovers, ref target3, hookCtx, false, context))
      target3 = this.AvailableTakeovers;
    target.AvailableTakeovers = target3;
    EntProtoId? target4 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.Prototype, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId?>(this.Prototype, hookCtx, context);
    target.Prototype = target4;
    List<string> target5 = (List<string>) null;
    if (this.SelectablePrototypes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.SelectablePrototypes, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<List<string>>(this.SelectablePrototypes, hookCtx, context);
    target.SelectablePrototypes = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GhostRoleMobSpawnerComponent target,
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
    GhostRoleMobSpawnerComponent target1 = (GhostRoleMobSpawnerComponent) target;
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
    GhostRoleMobSpawnerComponent target1 = (GhostRoleMobSpawnerComponent) target;
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
    GhostRoleMobSpawnerComponent target1 = (GhostRoleMobSpawnerComponent) target;
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
  virtual GhostRoleMobSpawnerComponent Component.Instantiate()
  {
    return new GhostRoleMobSpawnerComponent();
  }
}
