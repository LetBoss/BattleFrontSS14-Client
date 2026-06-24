// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Components.SlotBasedConnectedContainerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Containers;
using Content.Shared.Inventory;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Chemistry.Components;

[RegisterComponent]
[Access(new Type[] {typeof (SlotBasedConnectedContainerSystem)})]
[NetworkedComponent]
public sealed class SlotBasedConnectedContainerComponent : 
  Component,
  ISerializationGenerated<SlotBasedConnectedContainerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public SlotFlags TargetSlot;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? ContainerWhitelist;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SlotBasedConnectedContainerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (SlotBasedConnectedContainerComponent) component;
    if (serialization.TryCustomCopy<SlotBasedConnectedContainerComponent>(this, ref target, hookCtx, false, context))
      return;
    SlotFlags slotFlags = SlotFlags.NONE;
    if (!serialization.TryCustomCopy<SlotFlags>(this.TargetSlot, ref slotFlags, hookCtx, false, context))
      slotFlags = this.TargetSlot;
    target.TargetSlot = slotFlags;
    EntityWhitelist entityWhitelist = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.ContainerWhitelist, ref entityWhitelist, hookCtx, false, context))
    {
      if (this.ContainerWhitelist == null)
        entityWhitelist = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.ContainerWhitelist, ref entityWhitelist, hookCtx, context, false);
    }
    target.ContainerWhitelist = entityWhitelist;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SlotBasedConnectedContainerComponent target,
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
    SlotBasedConnectedContainerComponent target1 = (SlotBasedConnectedContainerComponent) target;
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
    SlotBasedConnectedContainerComponent target1 = (SlotBasedConnectedContainerComponent) target;
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
    SlotBasedConnectedContainerComponent target1 = (SlotBasedConnectedContainerComponent) target;
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
  virtual SlotBasedConnectedContainerComponent Component.Instantiate()
  {
    return new SlotBasedConnectedContainerComponent();
  }
}
