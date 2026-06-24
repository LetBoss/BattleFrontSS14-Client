// Decompiled with JetBrains decompiler
// Type: Content.Client.Storage.Visualizers.EntityStorageVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Storage.Visualizers;

[RegisterComponent]
[Access(new Type[] {typeof (EntityStorageVisualizerSystem)})]
public sealed class EntityStorageVisualsComponent : 
  Component,
  ISerializationGenerated<EntityStorageVisualsComponent>,
  ISerializationGenerated
{
  [DataField("stateBaseClosed", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string? StateBaseClosed;
  [DataField("stateBaseOpen", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string? StateBaseOpen;
  [DataField("stateDoorOpen", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string? StateDoorOpen;
  [DataField("stateDoorClosed", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string? StateDoorClosed;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public int? OpenDrawDepth;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public int? ClosedDrawDepth;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EntityStorageVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (EntityStorageVisualsComponent) component;
    if (serialization.TryCustomCopy<EntityStorageVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    string str1 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.StateBaseClosed, ref str1, hookCtx, false, context))
      str1 = this.StateBaseClosed;
    target.StateBaseClosed = str1;
    string str2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.StateBaseOpen, ref str2, hookCtx, false, context))
      str2 = this.StateBaseOpen;
    target.StateBaseOpen = str2;
    string str3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.StateDoorOpen, ref str3, hookCtx, false, context))
      str3 = this.StateDoorOpen;
    target.StateDoorOpen = str3;
    string str4 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.StateDoorClosed, ref str4, hookCtx, false, context))
      str4 = this.StateDoorClosed;
    target.StateDoorClosed = str4;
    int? nullable1 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.OpenDrawDepth, ref nullable1, hookCtx, false, context))
      nullable1 = this.OpenDrawDepth;
    target.OpenDrawDepth = nullable1;
    int? nullable2 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.ClosedDrawDepth, ref nullable2, hookCtx, false, context))
      nullable2 = this.ClosedDrawDepth;
    target.ClosedDrawDepth = nullable2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EntityStorageVisualsComponent target,
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
    EntityStorageVisualsComponent target1 = (EntityStorageVisualsComponent) target;
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
    EntityStorageVisualsComponent target1 = (EntityStorageVisualsComponent) target;
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
    EntityStorageVisualsComponent target1 = (EntityStorageVisualsComponent) target;
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
  virtual EntityStorageVisualsComponent Component.Instantiate()
  {
    return new EntityStorageVisualsComponent();
  }
}
