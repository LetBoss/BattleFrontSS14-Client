// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Components.DamageOnInteractProtectionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Damage.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class DamageOnInteractProtectionComponent : 
  Component,
  IClothingSlots,
  ISerializationGenerated<DamageOnInteractProtectionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public DamageModifierSet DamageProtection;

  [DataField(null, false, 1, false, false, null)]
  public SlotFlags Slots { get; set; } = SlotFlags.GLOVES;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DamageOnInteractProtectionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DamageOnInteractProtectionComponent) component;
    if (serialization.TryCustomCopy<DamageOnInteractProtectionComponent>(this, ref target, hookCtx, false, context))
      return;
    DamageModifierSet damageModifierSet = (DamageModifierSet) null;
    if (this.DamageProtection == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageModifierSet>(this.DamageProtection, ref damageModifierSet, hookCtx, true, context))
    {
      if (this.DamageProtection == null)
        damageModifierSet = (DamageModifierSet) null;
      else
        serialization.CopyTo<DamageModifierSet>(this.DamageProtection, ref damageModifierSet, hookCtx, context, true);
    }
    target.DamageProtection = damageModifierSet;
    SlotFlags slotFlags = SlotFlags.NONE;
    if (!serialization.TryCustomCopy<SlotFlags>(this.Slots, ref slotFlags, hookCtx, false, context))
      slotFlags = this.Slots;
    target.Slots = slotFlags;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DamageOnInteractProtectionComponent target,
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
    DamageOnInteractProtectionComponent target1 = (DamageOnInteractProtectionComponent) target;
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
    DamageOnInteractProtectionComponent target1 = (DamageOnInteractProtectionComponent) target;
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
    DamageOnInteractProtectionComponent target1 = (DamageOnInteractProtectionComponent) target;
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
  virtual DamageOnInteractProtectionComponent Component.Instantiate()
  {
    return new DamageOnInteractProtectionComponent();
  }
}
