// Decompiled with JetBrains decompiler
// Type: Content.Client.Trigger.TimerTriggerVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Animations;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Trigger;

[RegisterComponent]
[Access(new Type[] {typeof (TimerTriggerVisualizerSystem)})]
public sealed class TimerTriggerVisualsComponent : 
  Component,
  ISerializationGenerated<TimerTriggerVisualsComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public const string AnimationKey = "priming_animation";
  [DataField("unprimedSprite", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string UnprimedSprite = "icon";
  [DataField("primingSprite", false, 1, false, false, null)]
  public string PrimingSprite = "primed";
  [DataField("primingSound", false, 1, false, false, null)]
  public SoundSpecifier? PrimingSound;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Animation PrimingAnimation;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TimerTriggerVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (TimerTriggerVisualsComponent) component;
    if (serialization.TryCustomCopy<TimerTriggerVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    string str1 = (string) null;
    if (this.UnprimedSprite == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.UnprimedSprite, ref str1, hookCtx, false, context))
      str1 = this.UnprimedSprite;
    target.UnprimedSprite = str1;
    string str2 = (string) null;
    if (this.PrimingSprite == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.PrimingSprite, ref str2, hookCtx, false, context))
      str2 = this.PrimingSprite;
    target.PrimingSprite = str2;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.PrimingSound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.PrimingSound, hookCtx, context, false);
    target.PrimingSound = soundSpecifier;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TimerTriggerVisualsComponent target,
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
    TimerTriggerVisualsComponent target1 = (TimerTriggerVisualsComponent) target;
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
    TimerTriggerVisualsComponent target1 = (TimerTriggerVisualsComponent) target;
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
    TimerTriggerVisualsComponent target1 = (TimerTriggerVisualsComponent) target;
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
  virtual TimerTriggerVisualsComponent Component.Instantiate()
  {
    return new TimerTriggerVisualsComponent();
  }
}
