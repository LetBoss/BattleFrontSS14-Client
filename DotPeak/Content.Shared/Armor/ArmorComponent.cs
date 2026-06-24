// Decompiled with JetBrains decompiler
// Type: Content.Shared.Armor.ArmorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Armor;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedArmorSystem)})]
public sealed class ArmorComponent : 
  Component,
  ISerializationGenerated<ArmorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public DamageModifierSet Modifiers;
  [DataField(null, false, 1, false, false, null)]
  public float PriceMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public bool ShowArmorOnExamine = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ArmorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ArmorComponent) component;
    if (serialization.TryCustomCopy<ArmorComponent>(this, ref target, hookCtx, false, context))
      return;
    DamageModifierSet damageModifierSet = (DamageModifierSet) null;
    if (this.Modifiers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageModifierSet>(this.Modifiers, ref damageModifierSet, hookCtx, true, context))
    {
      if (this.Modifiers == null)
        damageModifierSet = (DamageModifierSet) null;
      else
        serialization.CopyTo<DamageModifierSet>(this.Modifiers, ref damageModifierSet, hookCtx, context, true);
    }
    target.Modifiers = damageModifierSet;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PriceMultiplier, ref num, hookCtx, false, context))
      num = this.PriceMultiplier;
    target.PriceMultiplier = num;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowArmorOnExamine, ref flag, hookCtx, false, context))
      flag = this.ShowArmorOnExamine;
    target.ShowArmorOnExamine = flag;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ArmorComponent target,
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
    ArmorComponent target1 = (ArmorComponent) target;
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
    ArmorComponent target1 = (ArmorComponent) target;
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
    ArmorComponent target1 = (ArmorComponent) target;
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
  virtual ArmorComponent Component.Instantiate() => new ArmorComponent();
}
