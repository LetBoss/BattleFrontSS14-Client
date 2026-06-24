// Decompiled with JetBrains decompiler
// Type: Content.Client.Storage.Components.StorageContainerVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Storage.Components;

[RegisterComponent]
public sealed class StorageContainerVisualsComponent : 
  Component,
  ISerializationGenerated<StorageContainerVisualsComponent>,
  ISerializationGenerated
{
  [DataField("maxFillLevels", false, 1, false, false, null)]
  public int MaxFillLevels;
  [DataField("fillBaseName", false, 1, false, false, null)]
  public string? FillBaseName;
  [DataField("layer", false, 1, false, false, null)]
  public StorageContainerVisualLayers FillLayer;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StorageContainerVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (StorageContainerVisualsComponent) component;
    if (serialization.TryCustomCopy<StorageContainerVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    int num = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxFillLevels, ref num, hookCtx, false, context))
      num = this.MaxFillLevels;
    target.MaxFillLevels = num;
    string str = (string) null;
    if (!serialization.TryCustomCopy<string>(this.FillBaseName, ref str, hookCtx, false, context))
      str = this.FillBaseName;
    target.FillBaseName = str;
    StorageContainerVisualLayers containerVisualLayers = StorageContainerVisualLayers.Fill;
    if (!serialization.TryCustomCopy<StorageContainerVisualLayers>(this.FillLayer, ref containerVisualLayers, hookCtx, false, context))
      containerVisualLayers = this.FillLayer;
    target.FillLayer = containerVisualLayers;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StorageContainerVisualsComponent target,
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
    StorageContainerVisualsComponent target1 = (StorageContainerVisualsComponent) target;
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
    StorageContainerVisualsComponent target1 = (StorageContainerVisualsComponent) target;
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
    StorageContainerVisualsComponent target1 = (StorageContainerVisualsComponent) target;
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
  virtual StorageContainerVisualsComponent Component.Instantiate()
  {
    return new StorageContainerVisualsComponent();
  }
}
