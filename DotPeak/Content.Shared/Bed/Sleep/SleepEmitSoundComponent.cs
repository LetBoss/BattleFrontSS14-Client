// Decompiled with JetBrains decompiler
// Type: Content.Shared.Bed.Sleep.SleepEmitSoundComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Bed.Sleep;

[RegisterComponent]
public sealed class SleepEmitSoundComponent : 
  Component,
  ISerializationGenerated<SleepEmitSoundComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public SoundSpecifier Snore = (SoundSpecifier) new SoundCollectionSpecifier("Snores", new AudioParams?(((AudioParams) ref AudioParams.Default).WithVariation(new float?(0.2f))));
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan Interval = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan MaxInterval = TimeSpan.FromSeconds(15L);
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public LocId PopUp = LocId.op_Implicit("sleep-onomatopoeia");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SleepEmitSoundComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (SleepEmitSoundComponent) component;
    if (serialization.TryCustomCopy<SleepEmitSoundComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (this.Snore == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Snore, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.Snore, hookCtx, context, false);
    target.Snore = soundSpecifier;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Interval, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.Interval, hookCtx, context, false);
    target.Interval = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MaxInterval, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.MaxInterval, hookCtx, context, false);
    target.MaxInterval = timeSpan2;
    LocId locId = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.PopUp, ref locId, hookCtx, false, context))
      locId = serialization.CreateCopy<LocId>(this.PopUp, hookCtx, context, false);
    target.PopUp = locId;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SleepEmitSoundComponent target,
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
    SleepEmitSoundComponent target1 = (SleepEmitSoundComponent) target;
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
    SleepEmitSoundComponent target1 = (SleepEmitSoundComponent) target;
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
    SleepEmitSoundComponent target1 = (SleepEmitSoundComponent) target;
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
  virtual SleepEmitSoundComponent Component.Instantiate() => new SleepEmitSoundComponent();
}
