// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.StationAi.StationAiHolderComponent
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
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Silicons.StationAi;

[RegisterComponent]
[NetworkedComponent]
public sealed class StationAiHolderComponent : 
  Component,
  ISerializationGenerated<StationAiHolderComponent>,
  ISerializationGenerated
{
  public const string Container = "station_ai_mind_slot";
  [DataField(null, false, 1, false, false, null)]
  public ItemSlot Slot = new ItemSlot();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StationAiHolderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StationAiHolderComponent) target1;
    if (serialization.TryCustomCopy<StationAiHolderComponent>(this, ref target, hookCtx, false, context))
      return;
    ItemSlot target2 = (ItemSlot) null;
    if (this.Slot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ItemSlot>(this.Slot, ref target2, hookCtx, false, context))
    {
      if (this.Slot == null)
        target2 = (ItemSlot) null;
      else
        serialization.CopyTo<ItemSlot>(this.Slot, ref target2, hookCtx, context, true);
    }
    target.Slot = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StationAiHolderComponent target,
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
    StationAiHolderComponent target1 = (StationAiHolderComponent) target;
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
    StationAiHolderComponent target1 = (StationAiHolderComponent) target;
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
    StationAiHolderComponent target1 = (StationAiHolderComponent) target;
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
  virtual StationAiHolderComponent Component.Instantiate() => new StationAiHolderComponent();
}
