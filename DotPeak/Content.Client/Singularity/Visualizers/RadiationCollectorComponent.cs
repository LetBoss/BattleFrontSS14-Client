// Decompiled with JetBrains decompiler
// Type: Content.Client.Singularity.Visualizers.RadiationCollectorComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Singularity.Components;
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
namespace Content.Client.Singularity.Visualizers;

[RegisterComponent]
[Access(new Type[] {typeof (RadiationCollectorSystem)})]
public sealed class RadiationCollectorComponent : 
  Component,
  ISerializationGenerated<RadiationCollectorComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public const string AnimationKey = "radiationcollector_animation";
  [Robust.Shared.ViewVariables.ViewVariables]
  public RadiationCollectorVisualState CurrentState;
  [DataField("activeState", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string ActiveState = "ca_on";
  [DataField("inactiveState", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string InactiveState = "ca_off";
  [DataField("activatingState", false, 1, false, false, null)]
  public string ActivatingState = "ca_active";
  [DataField("deactivatingState", false, 1, false, false, null)]
  public string DeactivatingState = "ca_deactive";
  [Robust.Shared.ViewVariables.ViewVariables]
  public Animation ActivateAnimation;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Animation DeactiveAnimation;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RadiationCollectorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (RadiationCollectorComponent) component;
    if (serialization.TryCustomCopy<RadiationCollectorComponent>(this, ref target, hookCtx, false, context))
      return;
    string str1 = (string) null;
    if (this.ActiveState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ActiveState, ref str1, hookCtx, false, context))
      str1 = this.ActiveState;
    target.ActiveState = str1;
    string str2 = (string) null;
    if (this.InactiveState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.InactiveState, ref str2, hookCtx, false, context))
      str2 = this.InactiveState;
    target.InactiveState = str2;
    string str3 = (string) null;
    if (this.ActivatingState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ActivatingState, ref str3, hookCtx, false, context))
      str3 = this.ActivatingState;
    target.ActivatingState = str3;
    string str4 = (string) null;
    if (this.DeactivatingState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DeactivatingState, ref str4, hookCtx, false, context))
      str4 = this.DeactivatingState;
    target.DeactivatingState = str4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RadiationCollectorComponent target,
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
    RadiationCollectorComponent target1 = (RadiationCollectorComponent) target;
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
    RadiationCollectorComponent target1 = (RadiationCollectorComponent) target;
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
    RadiationCollectorComponent target1 = (RadiationCollectorComponent) target;
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
  virtual RadiationCollectorComponent Component.Instantiate() => new RadiationCollectorComponent();
}
