// Decompiled with JetBrains decompiler
// Type: Content.Shared.Climbing.Components.GlassTableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Climbing.Systems;
using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Climbing.Components;

[RegisterComponent]
[Access(new Type[] {typeof (ClimbSystem)})]
public sealed class GlassTableComponent : 
  Component,
  ISerializationGenerated<GlassTableComponent>,
  ISerializationGenerated
{
  [DataField("climberDamage", false, 1, false, false, null)]
  public DamageSpecifier ClimberDamage;
  [DataField("tableDamage", false, 1, false, false, null)]
  public DamageSpecifier TableDamage;
  [DataField("tableMassLimit", false, 1, false, false, null)]
  public float MassLimit;
  public float StunTime = 2f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GlassTableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (GlassTableComponent) component;
    if (serialization.TryCustomCopy<GlassTableComponent>(this, ref target, hookCtx, false, context))
      return;
    DamageSpecifier damageSpecifier1 = (DamageSpecifier) null;
    if (this.ClimberDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.ClimberDamage, ref damageSpecifier1, hookCtx, false, context))
    {
      if (this.ClimberDamage == null)
        damageSpecifier1 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.ClimberDamage, ref damageSpecifier1, hookCtx, context, true);
    }
    target.ClimberDamage = damageSpecifier1;
    DamageSpecifier damageSpecifier2 = (DamageSpecifier) null;
    if (this.TableDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.TableDamage, ref damageSpecifier2, hookCtx, false, context))
    {
      if (this.TableDamage == null)
        damageSpecifier2 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.TableDamage, ref damageSpecifier2, hookCtx, context, true);
    }
    target.TableDamage = damageSpecifier2;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MassLimit, ref num, hookCtx, false, context))
      num = this.MassLimit;
    target.MassLimit = num;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GlassTableComponent target,
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
    GlassTableComponent target1 = (GlassTableComponent) target;
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
    GlassTableComponent target1 = (GlassTableComponent) target;
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
    GlassTableComponent target1 = (GlassTableComponent) target;
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
  virtual GlassTableComponent Component.Instantiate() => new GlassTableComponent();
}
