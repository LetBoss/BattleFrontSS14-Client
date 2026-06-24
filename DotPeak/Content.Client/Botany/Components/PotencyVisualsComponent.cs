// Decompiled with JetBrains decompiler
// Type: Content.Client.Botany.Components.PotencyVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Botany.Components;

[RegisterComponent]
public sealed class PotencyVisualsComponent : 
  Component,
  ISerializationGenerated<PotencyVisualsComponent>,
  ISerializationGenerated
{
  [DataField("minimumScale", false, 1, false, false, null)]
  public float MinimumScale = 1f;
  [DataField("maximumScale", false, 1, false, false, null)]
  public float MaximumScale = 2f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PotencyVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (PotencyVisualsComponent) component;
    if (serialization.TryCustomCopy<PotencyVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinimumScale, ref num1, hookCtx, false, context))
      num1 = this.MinimumScale;
    target.MinimumScale = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaximumScale, ref num2, hookCtx, false, context))
      num2 = this.MaximumScale;
    target.MaximumScale = num2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PotencyVisualsComponent target,
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
    PotencyVisualsComponent target1 = (PotencyVisualsComponent) target;
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
    PotencyVisualsComponent target1 = (PotencyVisualsComponent) target;
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
    PotencyVisualsComponent target1 = (PotencyVisualsComponent) target;
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
  virtual PotencyVisualsComponent Component.Instantiate() => new PotencyVisualsComponent();
}
