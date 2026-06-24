// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Components.MovedByPressureComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Atmos.Components;

[RegisterComponent]
public sealed class MovedByPressureComponent : 
  Component,
  ISerializationGenerated<MovedByPressureComponent>,
  ISerializationGenerated
{
  public const float MoveForcePushRatio = 1f;
  public const float MoveForceForcePushRatio = 1f;
  public const float ProbabilityOffset = 25f;
  public const float ProbabilityBasePercent = 10f;
  public const float ThrowForce = 100f;
  [DataField(null, false, 1, false, false, null)]
  public float Accumulator;
  [DataField(null, false, 1, false, false, null)]
  public HashSet<string> TableLayerRemoved = new HashSet<string>();

  [DataField(null, false, 1, false, false, null)]
  public bool Enabled { get; set; } = true;

  [DataField(null, false, 1, false, false, null)]
  public float PressureResistance { get; set; } = 1f;

  [DataField(null, false, 1, false, false, null)]
  public float MoveResist { get; set; } = 100f;

  [Robust.Shared.ViewVariables.ViewVariables]
  public int LastHighPressureMovementAirCycle { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MovedByPressureComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (MovedByPressureComponent) component;
    if (serialization.TryCustomCopy<MovedByPressureComponent>(this, ref target, hookCtx, false, context))
      return;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Accumulator, ref num1, hookCtx, false, context))
      num1 = this.Accumulator;
    target.Accumulator = num1;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref flag, hookCtx, false, context))
      flag = this.Enabled;
    target.Enabled = flag;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PressureResistance, ref num2, hookCtx, false, context))
      num2 = this.PressureResistance;
    target.PressureResistance = num2;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MoveResist, ref num3, hookCtx, false, context))
      num3 = this.MoveResist;
    target.MoveResist = num3;
    HashSet<string> stringSet = (HashSet<string>) null;
    if (this.TableLayerRemoved == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<string>>(this.TableLayerRemoved, ref stringSet, hookCtx, true, context))
      stringSet = serialization.CreateCopy<HashSet<string>>(this.TableLayerRemoved, hookCtx, context, false);
    target.TableLayerRemoved = stringSet;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MovedByPressureComponent target,
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
    MovedByPressureComponent target1 = (MovedByPressureComponent) target;
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
    MovedByPressureComponent target1 = (MovedByPressureComponent) target;
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
    MovedByPressureComponent target1 = (MovedByPressureComponent) target;
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
  virtual MovedByPressureComponent Component.Instantiate() => new MovedByPressureComponent();
}
