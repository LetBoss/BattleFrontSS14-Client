// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weather.RMCWeatherEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weather;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Weather;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class RMCWeatherEvent : 
  ISerializationGenerated<RMCWeatherEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string Name = "rmcWeatherEvent";
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Duration;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan DurationRemaining;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<WeatherPrototype> WeatherType;
  [DataField(null, false, 1, false, false, null)]
  public float LightningChance;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan LightningDuration = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan LightningCooldown;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan LightningCooldownDuration = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  public List<string> LightningEffects = new List<string>()
  {
    "RMCColorSequenceLightningSharpPeak",
    "RMCColorSequenceLightningFlicker"
  };
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier LightningSound = (SoundSpecifier) new SoundCollectionSpecifier("RMCThunder");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCWeatherEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<RMCWeatherEvent>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.Name == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Name, ref target1, hookCtx, false, context))
      target1 = this.Name;
    target.Name = target1;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DurationRemaining, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.DurationRemaining, hookCtx, context);
    target.DurationRemaining = target3;
    ProtoId<WeatherPrototype> target4 = new ProtoId<WeatherPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<WeatherPrototype>>(this.WeatherType, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<ProtoId<WeatherPrototype>>(this.WeatherType, hookCtx, context);
    target.WeatherType = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LightningChance, ref target5, hookCtx, false, context))
      target5 = this.LightningChance;
    target.LightningChance = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LightningDuration, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.LightningDuration, hookCtx, context);
    target.LightningDuration = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LightningCooldown, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.LightningCooldown, hookCtx, context);
    target.LightningCooldown = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LightningCooldownDuration, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.LightningCooldownDuration, hookCtx, context);
    target.LightningCooldownDuration = target8;
    List<string> target9 = (List<string>) null;
    if (this.LightningEffects == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.LightningEffects, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<List<string>>(this.LightningEffects, hookCtx, context);
    target.LightningEffects = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (this.LightningSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.LightningSound, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.LightningSound, hookCtx, context);
    target.LightningSound = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCWeatherEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCWeatherEvent target1 = (RMCWeatherEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public RMCWeatherEvent Instantiate() => new RMCWeatherEvent();
}
