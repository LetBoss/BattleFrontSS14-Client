// Decompiled with JetBrains decompiler
// Type: Content.Shared.CardboardBox.Components.CardboardBoxComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.CardboardBox.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class CardboardBoxComponent : 
  Component,
  ISerializationGenerated<CardboardBoxComponent>,
  ISerializationGenerated
{
  [DataField("mover", false, 1, false, false, null)]
  public EntityUid? Mover;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("effect", false, 1, false, false, null)]
  public string Effect = "Exclamation";
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("effectSound", false, 1, false, false, null)]
  public SoundSpecifier? EffectSound;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("quiet", false, 1, false, false, null)]
  public bool Quiet;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("distance", false, 1, false, false, null)]
  public float Distance = 6f;
  [DataField("effectCooldown", false, 1, false, false, typeof (TimeOffsetSerializer))]
  public TimeSpan EffectCooldown;
  [DataField("cooldownDuration", false, 1, false, false, null)]
  public TimeSpan CooldownDuration = TimeSpan.FromSeconds(5.0);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CardboardBoxComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (CardboardBoxComponent) component;
    if (serialization.TryCustomCopy<CardboardBoxComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? nullable = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Mover, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<EntityUid?>(this.Mover, hookCtx, context, false);
    target.Mover = nullable;
    string str = (string) null;
    if (this.Effect == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Effect, ref str, hookCtx, false, context))
      str = this.Effect;
    target.Effect = str;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.EffectSound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.EffectSound, hookCtx, context, false);
    target.EffectSound = soundSpecifier;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Quiet, ref flag, hookCtx, false, context))
      flag = this.Quiet;
    target.Quiet = flag;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Distance, ref num, hookCtx, false, context))
      num = this.Distance;
    target.Distance = num;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.EffectCooldown, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.EffectCooldown, hookCtx, context, false);
    target.EffectCooldown = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CooldownDuration, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.CooldownDuration, hookCtx, context, false);
    target.CooldownDuration = timeSpan2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CardboardBoxComponent target,
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
    CardboardBoxComponent target1 = (CardboardBoxComponent) target;
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
    CardboardBoxComponent target1 = (CardboardBoxComponent) target;
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
    CardboardBoxComponent target1 = (CardboardBoxComponent) target;
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
  virtual CardboardBoxComponent Component.Instantiate() => new CardboardBoxComponent();
}
