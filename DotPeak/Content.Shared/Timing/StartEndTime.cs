// Decompiled with JetBrains decompiler
// Type: Content.Shared.Timing.StartEndTime
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Timing;

[Serializable]
public record struct StartEndTime(TimeSpan Start, TimeSpan End)
{
  public TimeSpan Length => this.End - this.Start;

  public float ProgressAt(TimeSpan time, bool clamp = true)
  {
    TimeSpan length = this.Length;
    if (length == new TimeSpan())
      return float.NaN;
    float num = (float) ((time - this.Start) / length);
    if (clamp)
      num = MathHelper.Clamp01(num);
    return num;
  }

  public static StartEndTime FromStartDuration(TimeSpan start, TimeSpan duration)
  {
    return new StartEndTime(start, start + duration);
  }

  public static StartEndTime FromStartDuration(TimeSpan start, float durationSeconds)
  {
    return new StartEndTime(start, start + TimeSpan.FromSeconds((double) durationSeconds));
  }

  public static StartEndTime FromCurTime(IGameTiming gameTiming, TimeSpan duration)
  {
    return StartEndTime.FromStartDuration(gameTiming.CurTime, duration);
  }

  public static StartEndTime FromCurTime(IGameTiming gameTiming, float durationSeconds)
  {
    return StartEndTime.FromStartDuration(gameTiming.CurTime, durationSeconds);
  }
}
