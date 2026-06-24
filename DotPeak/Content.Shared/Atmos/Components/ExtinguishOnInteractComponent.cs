// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Components.ExtinguishOnInteractComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Atmos.Components;

[RegisterComponent]
public sealed class ExtinguishOnInteractComponent : 
  Component,
  ISerializationGenerated<ExtinguishOnInteractComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public SoundSpecifier? ExtinguishAttemptSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/candle_blowing.ogg", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float Probability = 0.9f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float StackDelta = -5f;
  [DataField(null, false, 1, false, false, null)]
  public LocId ExtinguishFailed = LocId.op_Implicit("candle-extinguish-failed");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ExtinguishOnInteractComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ExtinguishOnInteractComponent) component;
    if (serialization.TryCustomCopy<ExtinguishOnInteractComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ExtinguishAttemptSound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.ExtinguishAttemptSound, hookCtx, context, false);
    target.ExtinguishAttemptSound = soundSpecifier;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Probability, ref num1, hookCtx, false, context))
      num1 = this.Probability;
    target.Probability = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StackDelta, ref num2, hookCtx, false, context))
      num2 = this.StackDelta;
    target.StackDelta = num2;
    LocId locId = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.ExtinguishFailed, ref locId, hookCtx, false, context))
      locId = serialization.CreateCopy<LocId>(this.ExtinguishFailed, hookCtx, context, false);
    target.ExtinguishFailed = locId;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ExtinguishOnInteractComponent target,
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
    ExtinguishOnInteractComponent target1 = (ExtinguishOnInteractComponent) target;
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
    ExtinguishOnInteractComponent target1 = (ExtinguishOnInteractComponent) target;
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
    ExtinguishOnInteractComponent target1 = (ExtinguishOnInteractComponent) target;
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
  virtual ExtinguishOnInteractComponent Component.Instantiate()
  {
    return new ExtinguishOnInteractComponent();
  }
}
