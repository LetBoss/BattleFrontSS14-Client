// Decompiled with JetBrains decompiler
// Type: Content.Shared.Flash.Components.DamagedByFlashingComponent
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
namespace Content.Shared.Flash.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (DamagedByFlashingSystem)})]
public sealed class DamagedByFlashingComponent : 
  Component,
  ISerializationGenerated<DamagedByFlashingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public DamageSpecifier FlashDamage = new DamageSpecifier();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DamagedByFlashingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DamagedByFlashingComponent) target1;
    if (serialization.TryCustomCopy<DamagedByFlashingComponent>(this, ref target, hookCtx, false, context))
      return;
    DamageSpecifier target2 = (DamageSpecifier) null;
    if (this.FlashDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.FlashDamage, ref target2, hookCtx, false, context))
    {
      if (this.FlashDamage == null)
        target2 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.FlashDamage, ref target2, hookCtx, context, true);
    }
    target.FlashDamage = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DamagedByFlashingComponent target,
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
    DamagedByFlashingComponent target1 = (DamagedByFlashingComponent) target;
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
    DamagedByFlashingComponent target1 = (DamagedByFlashingComponent) target;
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
    DamagedByFlashingComponent target1 = (DamagedByFlashingComponent) target;
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
  virtual DamagedByFlashingComponent Component.Instantiate() => new DamagedByFlashingComponent();
}
