// Decompiled with JetBrains decompiler
// Type: Content.Shared.Timing.UseDelayInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Timing;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class UseDelayInfo : ISerializationGenerated<UseDelayInfo>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Length { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public TimeSpan StartTime { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public TimeSpan EndTime { get; set; }

  public UseDelayInfo(TimeSpan length, TimeSpan startTime = default (TimeSpan), TimeSpan endTime = default (TimeSpan))
  {
    this.Length = length;
    this.StartTime = startTime;
    this.EndTime = endTime;
  }

  public UseDelayInfo()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref UseDelayInfo target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<UseDelayInfo>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Length, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<TimeSpan>(this.Length, hookCtx, context);
    target.Length = target1;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StartTime, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.StartTime, hookCtx, context);
    target.StartTime = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.EndTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.EndTime, hookCtx, context);
    target.EndTime = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref UseDelayInfo target,
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
    UseDelayInfo target1 = (UseDelayInfo) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public UseDelayInfo Instantiate() => new UseDelayInfo();
}
