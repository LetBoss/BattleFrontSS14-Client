// Decompiled with JetBrains decompiler
// Type: Content.Shared.ComponentTable.ComponentTableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.EntityTable.EntitySelectors;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.ComponentTable;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedComponentTableSystem)})]
public sealed class ComponentTableComponent : 
  Component,
  ISerializationGenerated<ComponentTableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public EntityTableSelector Table;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ComponentTableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ComponentTableComponent) component;
    if (serialization.TryCustomCopy<ComponentTableComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityTableSelector entityTableSelector = (EntityTableSelector) null;
    if (this.Table == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityTableSelector>(this.Table, ref entityTableSelector, hookCtx, true, context))
      entityTableSelector = serialization.CreateCopy<EntityTableSelector>(this.Table, hookCtx, context, false);
    target.Table = entityTableSelector;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ComponentTableComponent target,
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
    ComponentTableComponent target1 = (ComponentTableComponent) target;
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
    ComponentTableComponent target1 = (ComponentTableComponent) target;
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
    ComponentTableComponent target1 = (ComponentTableComponent) target;
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
  virtual ComponentTableComponent Component.Instantiate() => new ComponentTableComponent();
}
