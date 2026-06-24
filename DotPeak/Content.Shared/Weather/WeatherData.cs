// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weather.WeatherData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;

#nullable enable
namespace Content.Shared.Weather;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class WeatherData : ISerializationGenerated<WeatherData>, ISerializationGenerated
{
  [NonSerialized]
  public EntityUid? Stream;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  public TimeSpan StartTime = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  public TimeSpan? EndTime;
  [DataField(null, false, 1, false, false, null)]
  public WeatherState State;

  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan Duration
  {
    get => this.EndTime.HasValue ? this.EndTime.Value - this.StartTime : TimeSpan.MaxValue;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WeatherData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<WeatherData>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StartTime, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<TimeSpan>(this.StartTime, hookCtx, context);
    target.StartTime = target1;
    TimeSpan? target2 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.EndTime, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan?>(this.EndTime, hookCtx, context);
    target.EndTime = target2;
    WeatherState target3 = WeatherState.Invalid;
    if (!serialization.TryCustomCopy<WeatherState>(this.State, ref target3, hookCtx, false, context))
      target3 = this.State;
    target.State = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WeatherData target,
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
    WeatherData target1 = (WeatherData) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public WeatherData Instantiate() => new WeatherData();
}
