// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.Components.ItemMapperComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Storage.Components;

[RegisterComponent]
[Access(new Type[] {typeof (SharedItemMapperSystem)})]
public sealed class ItemMapperComponent : 
  Component,
  ISerializationGenerated<ItemMapperComponent>,
  ISerializationGenerated
{
  [DataField("mapLayers", false, 1, false, false, null)]
  public Dictionary<string, SharedMapLayerData> MapLayers = new Dictionary<string, SharedMapLayerData>();
  [DataField("sprite", false, 1, false, false, null)]
  public ResPath? RSIPath;
  [DataField("containerWhitelist", false, 1, false, false, null)]
  public HashSet<string>? ContainerWhitelist;
  [DataField("spriteLayers", false, 1, false, false, null)]
  public List<string> SpriteLayers = new List<string>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ItemMapperComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ItemMapperComponent) target1;
    if (serialization.TryCustomCopy<ItemMapperComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<string, SharedMapLayerData> target2 = (Dictionary<string, SharedMapLayerData>) null;
    if (this.MapLayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, SharedMapLayerData>>(this.MapLayers, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<string, SharedMapLayerData>>(this.MapLayers, hookCtx, context);
    target.MapLayers = target2;
    ResPath? target3 = new ResPath?();
    if (!serialization.TryCustomCopy<ResPath?>(this.RSIPath, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ResPath?>(this.RSIPath, hookCtx, context);
    target.RSIPath = target3;
    HashSet<string> target4 = (HashSet<string>) null;
    if (!serialization.TryCustomCopy<HashSet<string>>(this.ContainerWhitelist, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<HashSet<string>>(this.ContainerWhitelist, hookCtx, context);
    target.ContainerWhitelist = target4;
    List<string> target5 = (List<string>) null;
    if (this.SpriteLayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.SpriteLayers, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<List<string>>(this.SpriteLayers, hookCtx, context);
    target.SpriteLayers = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ItemMapperComponent target,
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
    ItemMapperComponent target1 = (ItemMapperComponent) target;
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
    ItemMapperComponent target1 = (ItemMapperComponent) target;
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
    ItemMapperComponent target1 = (ItemMapperComponent) target;
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
  virtual ItemMapperComponent Component.Instantiate() => new ItemMapperComponent();
}
