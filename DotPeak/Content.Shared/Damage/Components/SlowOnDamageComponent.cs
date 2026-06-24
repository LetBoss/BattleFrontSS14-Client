// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Components.SlowOnDamageComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Damage.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class SlowOnDamageComponent : 
  Component,
  ISerializationGenerated<SlowOnDamageComponent>,
  ISerializationGenerated
{
  [DataField("speedModifierThresholds", false, 1, true, false, null)]
  public Dictionary<FixedPoint2, float> SpeedModifierThresholds;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SlowOnDamageComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (SlowOnDamageComponent) component;
    if (serialization.TryCustomCopy<SlowOnDamageComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<FixedPoint2, float> dictionary = (Dictionary<FixedPoint2, float>) null;
    if (this.SpeedModifierThresholds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<FixedPoint2, float>>(this.SpeedModifierThresholds, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<FixedPoint2, float>>(this.SpeedModifierThresholds, hookCtx, context, false);
    target.SpeedModifierThresholds = dictionary;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SlowOnDamageComponent target,
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
    SlowOnDamageComponent target1 = (SlowOnDamageComponent) target;
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
    SlowOnDamageComponent target1 = (SlowOnDamageComponent) target;
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
    SlowOnDamageComponent target1 = (SlowOnDamageComponent) target;
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
  virtual SlowOnDamageComponent Component.Instantiate() => new SlowOnDamageComponent();
}
