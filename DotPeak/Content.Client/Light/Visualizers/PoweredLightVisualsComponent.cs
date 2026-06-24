// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.Visualizers.PoweredLightVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Light;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Light.Visualizers;

[RegisterComponent]
[Access(new Type[] {typeof (PoweredLightVisualizerSystem)})]
public sealed class PoweredLightVisualsComponent : 
  Component,
  ISerializationGenerated<PoweredLightVisualsComponent>,
  ISerializationGenerated
{
  [DataField("spriteStateMap", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public Dictionary<PoweredLightState, string> SpriteStateMap = new Dictionary<PoweredLightState, string>()
  {
    [PoweredLightState.Empty] = "empty",
    [PoweredLightState.Off] = "off",
    [PoweredLightState.On] = "on",
    [PoweredLightState.Broken] = "broken",
    [PoweredLightState.Burned] = "burn"
  };
  [Robust.Shared.ViewVariables.ViewVariables]
  public const string BlinkingAnimationKey = "poweredlight_blinking";
  [DataField("minBlinkingTime", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float MinBlinkingAnimationCycleTime = 0.5f;
  [DataField("maxBlinkingTime", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float MaxBlinkingAnimationCycleTime = 2f;
  [DataField("blinkingSound", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public SoundSpecifier? BlinkingSound;
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool IsBlinking;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PoweredLightVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (PoweredLightVisualsComponent) component;
    if (serialization.TryCustomCopy<PoweredLightVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<PoweredLightState, string> dictionary = (Dictionary<PoweredLightState, string>) null;
    if (this.SpriteStateMap == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<PoweredLightState, string>>(this.SpriteStateMap, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<PoweredLightState, string>>(this.SpriteStateMap, hookCtx, context, false);
    target.SpriteStateMap = dictionary;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinBlinkingAnimationCycleTime, ref num1, hookCtx, false, context))
      num1 = this.MinBlinkingAnimationCycleTime;
    target.MinBlinkingAnimationCycleTime = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxBlinkingAnimationCycleTime, ref num2, hookCtx, false, context))
      num2 = this.MaxBlinkingAnimationCycleTime;
    target.MaxBlinkingAnimationCycleTime = num2;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BlinkingSound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.BlinkingSound, hookCtx, context, false);
    target.BlinkingSound = soundSpecifier;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PoweredLightVisualsComponent target,
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
    PoweredLightVisualsComponent target1 = (PoweredLightVisualsComponent) target;
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
    PoweredLightVisualsComponent target1 = (PoweredLightVisualsComponent) target;
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
    PoweredLightVisualsComponent target1 = (PoweredLightVisualsComponent) target;
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
  virtual PoweredLightVisualsComponent Component.Instantiate()
  {
    return new PoweredLightVisualsComponent();
  }
}
