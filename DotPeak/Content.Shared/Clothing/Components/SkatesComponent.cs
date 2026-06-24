// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.Components.SkatesComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Clothing.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SkatesSystem)})]
public sealed class SkatesComponent : 
  Component,
  ISerializationGenerated<SkatesComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float Friction = 0.125f;
  [DataField(null, false, 1, false, false, null)]
  public float FrictionNoInput = 0.125f;
  [DataField(null, false, 1, false, false, null)]
  public float Acceleration = 0.25f;
  [DataField(null, false, 1, false, false, null)]
  public float MinimumSpeed = 3f;
  [DataField(null, false, 1, false, false, null)]
  public float StunSeconds = 3f;
  [DataField(null, false, 1, false, false, null)]
  public float DamageCooldown = 2f;
  [DataField(null, false, 1, false, false, null)]
  public float SpeedDamage = 1f;
  [Robust.Shared.ViewVariables.ViewVariables]
  public float DefaultMinimumSpeed = 20f;
  [Robust.Shared.ViewVariables.ViewVariables]
  public float DefaultStunSeconds = 1f;
  [Robust.Shared.ViewVariables.ViewVariables]
  public float DefaultDamageCooldown = 2f;
  [Robust.Shared.ViewVariables.ViewVariables]
  public float DefaultSpeedDamage = 0.5f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SkatesComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (SkatesComponent) component;
    if (serialization.TryCustomCopy<SkatesComponent>(this, ref target, hookCtx, false, context))
      return;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Friction, ref num1, hookCtx, false, context))
      num1 = this.Friction;
    target.Friction = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FrictionNoInput, ref num2, hookCtx, false, context))
      num2 = this.FrictionNoInput;
    target.FrictionNoInput = num2;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Acceleration, ref num3, hookCtx, false, context))
      num3 = this.Acceleration;
    target.Acceleration = num3;
    float num4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinimumSpeed, ref num4, hookCtx, false, context))
      num4 = this.MinimumSpeed;
    target.MinimumSpeed = num4;
    float num5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StunSeconds, ref num5, hookCtx, false, context))
      num5 = this.StunSeconds;
    target.StunSeconds = num5;
    float num6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DamageCooldown, ref num6, hookCtx, false, context))
      num6 = this.DamageCooldown;
    target.DamageCooldown = num6;
    float num7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpeedDamage, ref num7, hookCtx, false, context))
      num7 = this.SpeedDamage;
    target.SpeedDamage = num7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SkatesComponent target,
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
    SkatesComponent target1 = (SkatesComponent) target;
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
    SkatesComponent target1 = (SkatesComponent) target;
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
    SkatesComponent target1 = (SkatesComponent) target;
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
  virtual SkatesComponent Component.Instantiate() => new SkatesComponent();
}
