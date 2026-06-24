// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.Components.ItemCounterComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Storage.EntitySystems;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Storage.Components;

[RegisterComponent]
[Access(new Type[] {typeof (SharedItemCounterSystem)})]
public sealed class ItemCounterComponent : 
  Component,
  ISerializationGenerated<ItemCounterComponent>,
  ISerializationGenerated
{
  [DataField("baseLayer", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string BaseLayer = "";
  [DataField("composite", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool IsComposite;
  [DataField("layerStates", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public List<string> LayerStates = new List<string>();

  [DataField("count", false, 1, true, false, null)]
  public EntityWhitelist Count { get; set; }

  [DataField("amount", false, 1, false, false, null)]
  public int? MaxAmount { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ItemCounterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ItemCounterComponent) target1;
    if (serialization.TryCustomCopy<ItemCounterComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist target2 = (EntityWhitelist) null;
    if (this.Count == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Count, ref target2, hookCtx, false, context))
    {
      if (this.Count == null)
        target2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Count, ref target2, hookCtx, context, true);
    }
    target.Count = target2;
    int? target3 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.MaxAmount, ref target3, hookCtx, false, context))
      target3 = this.MaxAmount;
    target.MaxAmount = target3;
    string target4 = (string) null;
    if (this.BaseLayer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BaseLayer, ref target4, hookCtx, false, context))
      target4 = this.BaseLayer;
    target.BaseLayer = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsComposite, ref target5, hookCtx, false, context))
      target5 = this.IsComposite;
    target.IsComposite = target5;
    List<string> target6 = (List<string>) null;
    if (this.LayerStates == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.LayerStates, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<List<string>>(this.LayerStates, hookCtx, context);
    target.LayerStates = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ItemCounterComponent target,
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
    ItemCounterComponent target1 = (ItemCounterComponent) target;
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
    ItemCounterComponent target1 = (ItemCounterComponent) target;
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
    ItemCounterComponent target1 = (ItemCounterComponent) target;
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
  virtual ItemCounterComponent Component.Instantiate() => new ItemCounterComponent();
}
