// Decompiled with JetBrains decompiler
// Type: Content.Shared.Strip.Components.StrippableDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Strip.Components;

[NetSerializable]
[Serializable]
public sealed class StrippableDoAfterEvent : 
  DoAfterEvent,
  ISerializationGenerated<StrippableDoAfterEvent>,
  ISerializationGenerated
{
  public readonly bool InsertOrRemove;
  public readonly bool InventoryOrHand;
  public readonly string SlotOrHandName;

  public StrippableDoAfterEvent(bool insertOrRemove, bool inventoryOrHand, string slotOrHandName)
  {
    this.InsertOrRemove = insertOrRemove;
    this.InventoryOrHand = inventoryOrHand;
    this.SlotOrHandName = slotOrHandName;
  }

  public override DoAfterEvent Clone() => (DoAfterEvent) this;

  public StrippableDoAfterEvent()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StrippableDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DoAfterEvent target1 = (DoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StrippableDoAfterEvent) target1;
    serialization.TryCustomCopy<StrippableDoAfterEvent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StrippableDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref DoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    StrippableDoAfterEvent target1 = (StrippableDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (DoAfterEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    StrippableDoAfterEvent target1 = (StrippableDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual StrippableDoAfterEvent DoAfterEvent.Instantiate() => new StrippableDoAfterEvent();
}
