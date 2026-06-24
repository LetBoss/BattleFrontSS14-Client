// Decompiled with JetBrains decompiler
// Type: Content.Shared.Anomaly.Effects.Components.PyroclasticAnomalyComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Anomaly.Effects.Components;

[RegisterComponent]
public sealed class PyroclasticAnomalyComponent : 
  Component,
  ISerializationGenerated<PyroclasticAnomalyComponent>,
  ISerializationGenerated
{
  [DataField("maximumIgnitionRadius", false, 1, false, false, null)]
  public float MaximumIgnitionRadius = 5f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PyroclasticAnomalyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (PyroclasticAnomalyComponent) component;
    if (serialization.TryCustomCopy<PyroclasticAnomalyComponent>(this, ref target, hookCtx, false, context))
      return;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaximumIgnitionRadius, ref num, hookCtx, false, context))
      num = this.MaximumIgnitionRadius;
    target.MaximumIgnitionRadius = num;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PyroclasticAnomalyComponent target,
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
    PyroclasticAnomalyComponent target1 = (PyroclasticAnomalyComponent) target;
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
    PyroclasticAnomalyComponent target1 = (PyroclasticAnomalyComponent) target;
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
    PyroclasticAnomalyComponent target1 = (PyroclasticAnomalyComponent) target;
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
  virtual PyroclasticAnomalyComponent Component.Instantiate() => new PyroclasticAnomalyComponent();
}
