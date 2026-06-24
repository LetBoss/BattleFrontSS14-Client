// Decompiled with JetBrains decompiler
// Type: Content.Client.Chemistry.Visualizers.VaporVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Animations;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Chemistry.Visualizers;

[RegisterComponent]
[Access(new Type[] {typeof (VaporVisualizerSystem)})]
public sealed class VaporVisualsComponent : 
  Component,
  ISerializationGenerated<VaporVisualsComponent>,
  ISerializationGenerated
{
  public const string AnimationKey = "flick_animation";
  [DataField("animationTime", false, 1, false, false, null)]
  public float AnimationTime = 0.25f;
  [DataField("animationState", false, 1, false, false, null)]
  public string AnimationState = "chempuff";
  public Animation VaporFlick;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VaporVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (VaporVisualsComponent) component;
    if (serialization.TryCustomCopy<VaporVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AnimationTime, ref num, hookCtx, false, context))
      num = this.AnimationTime;
    target.AnimationTime = num;
    string str = (string) null;
    if (this.AnimationState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.AnimationState, ref str, hookCtx, false, context))
      str = this.AnimationState;
    target.AnimationState = str;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VaporVisualsComponent target,
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
    VaporVisualsComponent target1 = (VaporVisualsComponent) target;
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
    VaporVisualsComponent target1 = (VaporVisualsComponent) target;
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
    VaporVisualsComponent target1 = (VaporVisualsComponent) target;
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
  virtual VaporVisualsComponent Component.Instantiate() => new VaporVisualsComponent();
}
