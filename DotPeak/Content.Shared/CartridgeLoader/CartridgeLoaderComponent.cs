// Decompiled with JetBrains decompiler
// Type: Content.Shared.CartridgeLoader.CartridgeLoaderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Containers.ItemSlots;
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
namespace Content.Shared.CartridgeLoader;

[RegisterComponent]
[NetworkedComponent]
public sealed class CartridgeLoaderComponent : 
  Component,
  ISerializationGenerated<CartridgeLoaderComponent>,
  ISerializationGenerated
{
  public const string CartridgeSlotId = "Cartridge-Slot";
  [DataField(null, false, 1, false, false, null)]
  public ItemSlot CartridgeSlot = new ItemSlot();
  [DataField("preinstalled", false, 1, false, false, null)]
  public List<string> PreinstalledPrograms = new List<string>();
  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid? ActiveProgram;
  [Robust.Shared.ViewVariables.ViewVariables]
  public readonly List<EntityUid> BackgroundPrograms = new List<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  public int DiskSpace = 8;
  [DataField(null, false, 1, false, false, null)]
  public bool NotificationsEnabled = true;
  [DataField(null, false, 1, true, false, null)]
  public Enum UiKey;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CartridgeLoaderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (CartridgeLoaderComponent) component;
    if (serialization.TryCustomCopy<CartridgeLoaderComponent>(this, ref target, hookCtx, false, context))
      return;
    ItemSlot itemSlot = (ItemSlot) null;
    if (this.CartridgeSlot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ItemSlot>(this.CartridgeSlot, ref itemSlot, hookCtx, false, context))
    {
      if (this.CartridgeSlot == null)
        itemSlot = (ItemSlot) null;
      else
        serialization.CopyTo<ItemSlot>(this.CartridgeSlot, ref itemSlot, hookCtx, context, true);
    }
    target.CartridgeSlot = itemSlot;
    List<string> stringList = (List<string>) null;
    if (this.PreinstalledPrograms == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.PreinstalledPrograms, ref stringList, hookCtx, true, context))
      stringList = serialization.CreateCopy<List<string>>(this.PreinstalledPrograms, hookCtx, context, false);
    target.PreinstalledPrograms = stringList;
    int num = 0;
    if (!serialization.TryCustomCopy<int>(this.DiskSpace, ref num, hookCtx, false, context))
      num = this.DiskSpace;
    target.DiskSpace = num;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.NotificationsEnabled, ref flag, hookCtx, false, context))
      flag = this.NotificationsEnabled;
    target.NotificationsEnabled = flag;
    Enum @enum = (Enum) null;
    if (this.UiKey == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Enum>(this.UiKey, ref @enum, hookCtx, true, context))
      @enum = this.UiKey;
    target.UiKey = @enum;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CartridgeLoaderComponent target,
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
    CartridgeLoaderComponent target1 = (CartridgeLoaderComponent) target;
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
    CartridgeLoaderComponent target1 = (CartridgeLoaderComponent) target;
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
    CartridgeLoaderComponent target1 = (CartridgeLoaderComponent) target;
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
  virtual CartridgeLoaderComponent Component.Instantiate() => new CartridgeLoaderComponent();
}
