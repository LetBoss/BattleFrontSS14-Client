// Decompiled with JetBrains decompiler
// Type: Content.Shared.Explosion.Components.ActiveTimerTriggerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Explosion.Components;

[RegisterComponent]
public sealed class ActiveTimerTriggerComponent : 
  Component,
  ISerializationGenerated<ActiveTimerTriggerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float TimeRemaining;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? User;
  [DataField(null, false, 1, false, false, null)]
  public float BeepInterval;
  [DataField(null, false, 1, false, false, null)]
  public float TimeUntilBeep;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? BeepSound;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ActiveTimerTriggerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ActiveTimerTriggerComponent) target1;
    if (serialization.TryCustomCopy<ActiveTimerTriggerComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TimeRemaining, ref target2, hookCtx, false, context))
      target2 = this.TimeRemaining;
    target.TimeRemaining = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.User, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.User, hookCtx, context);
    target.User = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BeepInterval, ref target4, hookCtx, false, context))
      target4 = this.BeepInterval;
    target.BeepInterval = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TimeUntilBeep, ref target5, hookCtx, false, context))
      target5 = this.TimeUntilBeep;
    target.TimeUntilBeep = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BeepSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.BeepSound, hookCtx, context);
    target.BeepSound = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ActiveTimerTriggerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ActiveTimerTriggerComponent target1 = (ActiveTimerTriggerComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ActiveTimerTriggerComponent target1 = (ActiveTimerTriggerComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ActiveTimerTriggerComponent target1 = (ActiveTimerTriggerComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ActiveTimerTriggerComponent Component.Instantiate() => new ActiveTimerTriggerComponent();
}
