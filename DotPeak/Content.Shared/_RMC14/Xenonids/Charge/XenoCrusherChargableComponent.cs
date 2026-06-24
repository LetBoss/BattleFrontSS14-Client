// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Charge.XenoCrusherChargableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Charge;

[RegisterComponent]
[NetworkedComponent]
public sealed class XenoCrusherChargableComponent : 
  Component,
  ISerializationGenerated<XenoCrusherChargableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool InstantDestroy;
  [DataField(null, false, 1, false, false, null)]
  public bool PassOnDestroy;
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier? SetDamage;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2? DestroyDamage;
  [DataField(null, false, 1, false, false, null)]
  public float? ThrowRange;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoCrusherChargableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoCrusherChargableComponent) target1;
    if (serialization.TryCustomCopy<XenoCrusherChargableComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.InstantDestroy, ref target2, hookCtx, false, context))
      target2 = this.InstantDestroy;
    target.InstantDestroy = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.PassOnDestroy, ref target3, hookCtx, false, context))
      target3 = this.PassOnDestroy;
    target.PassOnDestroy = target3;
    DamageSpecifier target4 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.SetDamage, ref target4, hookCtx, false, context))
    {
      if (this.SetDamage == null)
        target4 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.SetDamage, ref target4, hookCtx, context);
    }
    target.SetDamage = target4;
    FixedPoint2? target5 = new FixedPoint2?();
    if (!serialization.TryCustomCopy<FixedPoint2?>(this.DestroyDamage, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<FixedPoint2?>(this.DestroyDamage, hookCtx, context);
    target.DestroyDamage = target5;
    float? target6 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.ThrowRange, ref target6, hookCtx, false, context))
      target6 = this.ThrowRange;
    target.ThrowRange = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoCrusherChargableComponent target,
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
    XenoCrusherChargableComponent target1 = (XenoCrusherChargableComponent) target;
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
    XenoCrusherChargableComponent target1 = (XenoCrusherChargableComponent) target;
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
    XenoCrusherChargableComponent target1 = (XenoCrusherChargableComponent) target;
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
  virtual XenoCrusherChargableComponent Component.Instantiate()
  {
    return new XenoCrusherChargableComponent();
  }
}
