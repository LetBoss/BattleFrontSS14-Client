// Decompiled with JetBrains decompiler
// Type: Content.Shared.Anomaly.Effects.Components.EntitySpawnAnomalyComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Anomaly.Effects.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedEntityAnomalySystem)})]
public sealed class EntitySpawnAnomalyComponent : 
  Component,
  ISerializationGenerated<EntitySpawnAnomalyComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public List<EntitySpawnSettingsEntry> Entries = new List<EntitySpawnSettingsEntry>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EntitySpawnAnomalyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (EntitySpawnAnomalyComponent) component;
    if (serialization.TryCustomCopy<EntitySpawnAnomalyComponent>(this, ref target, hookCtx, false, context))
      return;
    List<EntitySpawnSettingsEntry> spawnSettingsEntryList = (List<EntitySpawnSettingsEntry>) null;
    if (this.Entries == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntitySpawnSettingsEntry>>(this.Entries, ref spawnSettingsEntryList, hookCtx, true, context))
      spawnSettingsEntryList = serialization.CreateCopy<List<EntitySpawnSettingsEntry>>(this.Entries, hookCtx, context, false);
    target.Entries = spawnSettingsEntryList;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EntitySpawnAnomalyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntitySpawnAnomalyComponent target1 = (EntitySpawnAnomalyComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntitySpawnAnomalyComponent target1 = (EntitySpawnAnomalyComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntitySpawnAnomalyComponent target1 = (EntitySpawnAnomalyComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual EntitySpawnAnomalyComponent Component.Instantiate() => new EntitySpawnAnomalyComponent();
}
