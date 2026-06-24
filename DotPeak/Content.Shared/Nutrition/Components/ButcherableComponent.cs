// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.Components.ButcherableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Storage;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Nutrition.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class ButcherableComponent : 
  Component,
  ISerializationGenerated<ButcherableComponent>,
  ISerializationGenerated
{
  [DataField("spawned", false, 1, true, false, null)]
  public List<EntitySpawnEntry> SpawnedEntities = new List<EntitySpawnEntry>();
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("butcherDelay", false, 1, false, false, null)]
  public float ButcherDelay = 8f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("butcheringType", false, 1, false, false, null)]
  public ButcheringType Type;
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool BeingButchered;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ButcherableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ButcherableComponent) target1;
    if (serialization.TryCustomCopy<ButcherableComponent>(this, ref target, hookCtx, false, context))
      return;
    List<EntitySpawnEntry> target2 = (List<EntitySpawnEntry>) null;
    if (this.SpawnedEntities == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntitySpawnEntry>>(this.SpawnedEntities, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<EntitySpawnEntry>>(this.SpawnedEntities, hookCtx, context);
    target.SpawnedEntities = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ButcherDelay, ref target3, hookCtx, false, context))
      target3 = this.ButcherDelay;
    target.ButcherDelay = target3;
    ButcheringType target4 = ButcheringType.Knife;
    if (!serialization.TryCustomCopy<ButcheringType>(this.Type, ref target4, hookCtx, false, context))
      target4 = this.Type;
    target.Type = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ButcherableComponent target,
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
    ButcherableComponent target1 = (ButcherableComponent) target;
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
    ButcherableComponent target1 = (ButcherableComponent) target;
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
    ButcherableComponent target1 = (ButcherableComponent) target;
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
  virtual ButcherableComponent Component.Instantiate() => new ButcherableComponent();
}
