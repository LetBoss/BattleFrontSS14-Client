// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.Borgs.Components.ItemBorgModuleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Silicons.Borgs.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedBorgSystem)})]
public sealed class ItemBorgModuleComponent : 
  Component,
  ISerializationGenerated<ItemBorgModuleComponent>,
  ISerializationGenerated
{
  [DataField("items", false, 1, true, false, typeof (PrototypeIdListSerializer<EntityPrototype>))]
  public System.Collections.Generic.List<string> Items = new System.Collections.Generic.List<string>();
  [DataField("providedItems", false, 1, false, false, null)]
  public SortedDictionary<string, EntityUid> ProvidedItems = new SortedDictionary<string, EntityUid>();
  [DataField("handCounter", false, 1, false, false, null)]
  public int HandCounter;
  [DataField("itemsCrated", false, 1, false, false, null)]
  public bool ItemsCreated;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Container ProvidedContainer;
  [DataField("providedContainerId", false, 1, false, false, null)]
  public string ProvidedContainerId = "provided_container";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ItemBorgModuleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ItemBorgModuleComponent) target1;
    if (serialization.TryCustomCopy<ItemBorgModuleComponent>(this, ref target, hookCtx, false, context))
      return;
    System.Collections.Generic.List<string> target2 = (System.Collections.Generic.List<string>) null;
    if (this.Items == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.List<string>>(this.Items, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<System.Collections.Generic.List<string>>(this.Items, hookCtx, context);
    target.Items = target2;
    SortedDictionary<string, EntityUid> target3 = (SortedDictionary<string, EntityUid>) null;
    if (this.ProvidedItems == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SortedDictionary<string, EntityUid>>(this.ProvidedItems, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SortedDictionary<string, EntityUid>>(this.ProvidedItems, hookCtx, context);
    target.ProvidedItems = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.HandCounter, ref target4, hookCtx, false, context))
      target4 = this.HandCounter;
    target.HandCounter = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.ItemsCreated, ref target5, hookCtx, false, context))
      target5 = this.ItemsCreated;
    target.ItemsCreated = target5;
    string target6 = (string) null;
    if (this.ProvidedContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ProvidedContainerId, ref target6, hookCtx, false, context))
      target6 = this.ProvidedContainerId;
    target.ProvidedContainerId = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ItemBorgModuleComponent target,
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
    ItemBorgModuleComponent target1 = (ItemBorgModuleComponent) target;
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
    ItemBorgModuleComponent target1 = (ItemBorgModuleComponent) target;
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
    ItemBorgModuleComponent target1 = (ItemBorgModuleComponent) target;
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
  virtual ItemBorgModuleComponent Component.Instantiate() => new ItemBorgModuleComponent();
}
