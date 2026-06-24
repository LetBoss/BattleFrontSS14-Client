// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Timing.GameTiming
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Timing;

[Virtual]
public class GameTiming : IGameTiming
{
  private const int NumFrames = 60;
  private readonly IStopwatch _realTimer = (IStopwatch) new Stopwatch();
  private readonly long[] _realFrameTimes = new long[60];
  private int _frameIdx;
  private TimeSpan _lastRealTime;
  private ushort _tickRate;
  private TimeSpan _tickRemainder;

  public GameTiming()
  {
    this._realTimer.Start();
    this.Paused = true;
    this.TickRate = (ushort) 60;
  }

  public bool InSimulation { get; set; }

  public bool Paused { get; set; }

  public (TimeSpan, GameTick) TimeBase { get; set; } = (TimeSpan.Zero, GameTick.First);

  public TimeSpan CurTime
  {
    get
    {
      (TimeSpan, GameTick) timeBase = this.TimeBase;
      TimeSpan timeSpan = timeBase.Item1 + this.TickPeriod.Mul((long) (this.CurTick.Value - timeBase.Item2.Value));
      return !this.InSimulation ? timeSpan + this.TickRemainder : timeSpan;
    }
  }

  public TimeSpan RealTime => this._realTimer.Elapsed;

  public TimeSpan FrameStartTime => this._lastRealTime;

  public virtual TimeSpan ServerTime => TimeSpan.Zero;

  public TimeSpan FrameTime => this.CalcFrameTime();

  public TimeSpan RealFrameTime { get; private set; }

  public TimeSpan RealFrameTimeAvg
  {
    get => TimeSpan.FromTicks((long) ((IEnumerable<long>) this._realFrameTimes).Average());
  }

  public TimeSpan RealFrameTimeStdDev => this.CalcRftStdDev();

  public double FramesPerSecondAvg => this.CalcFpsAvg();

  public GameTick CurTick { get; set; } = new GameTick(1U);

  public TimeSpan LastTick { get; set; }

  public ushort TickRate
  {
    get => this._tickRate;
    set => this.SetTickRateAt(value, this.CurTick);
  }

  public float TimeScale { get; set; } = 1f;

  public TimeSpan TickPeriod
  {
    get => TimeSpan.FromTicks((long) (1.0 / (double) this.TickRate * 10000000.0));
  }

  public TimeSpan TickRemainder
  {
    get => this._tickRemainder;
    set => this._tickRemainder = value;
  }

  public TimeSpan TickRemainderRealtime => this.TickRemainder * (double) this.TimeScale;

  public TimeSpan CalcAdjustedTickPeriod()
  {
    return this.TickPeriod * (1.0 - (double) MathHelper.Clamp(this.TickTimingAdjustment, -0.99f, 0.99f)) * (double) this.TimeScale;
  }

  public uint CurFrame { get; set; } = 1;

  public float TickTimingAdjustment { get; set; }

  public void StartFrame()
  {
    TimeSpan realTime = this.RealTime;
    this.RealFrameTime = realTime - this._lastRealTime;
    this._lastRealTime = realTime;
    this._frameIdx = (1 + this._frameIdx) % this._realFrameTimes.Length;
    this._realFrameTimes[this._frameIdx] = this.RealFrameTime.Ticks;
  }

  private TimeSpan CalcFrameTime()
  {
    if (this.InSimulation)
      return TimeSpan.FromTicks(this.TickPeriod.Ticks);
    return !this.Paused ? this.RealFrameTime : TimeSpan.Zero;
  }

  private void CacheCurTime(GameTick atTick)
  {
    (TimeSpan timeSpan, GameTick gameTick) = this.TimeBase;
    this.TimeBase = (timeSpan + this.TickPeriod.Mul((long) (atTick.Value - gameTick.Value)), atTick);
  }

  public void ResetSimTime() => this.ResetSimTime((TimeSpan.Zero, GameTick.First));

  public void ResetSimTime((TimeSpan, GameTick) timeBase)
  {
    this.TimeBase = timeBase;
    this.CurTick = GameTick.First;
    this.TickRemainder = TimeSpan.Zero;
    this.Paused = true;
  }

  public void SetTickRateAt(ushort tickRate, GameTick atTick)
  {
    if (this._tickRate != (ushort) 0)
      this.CacheCurTime(atTick);
    this._tickRate = tickRate;
  }

  public virtual TimeSpan RealLocalToServer(TimeSpan local) => TimeSpan.Zero;

  public virtual TimeSpan RealServerToLocal(TimeSpan server) => TimeSpan.Zero;

  protected virtual TimeSpan? GetServerOffset() => new TimeSpan?();

  public bool IsFirstTimePredicted { get; protected set; } = true;

  public virtual bool InPrediction => false;

  public bool ApplyingState { get; protected set; }

  private double CalcFpsAvg()
  {
    return 1.0 / (((IEnumerable<long>) this._realFrameTimes).Average() / 10000000.0);
  }

  private TimeSpan CalcRftStdDev()
  {
    long num1 = ((IEnumerable<long>) this._realFrameTimes).Sum();
    int length = this._realFrameTimes.Length;
    double num2 = (double) num1 / (double) length;
    double num3 = 0.0;
    for (int index = 0; index < length; ++index)
    {
      if (this._realFrameTimes[index] != 0L)
      {
        double num4 = (double) this._realFrameTimes[index] - num2;
        num3 += num4 * num4;
      }
    }
    return TimeSpan.FromTicks((long) Math.Sqrt(num3 / (double) (length - 1)));
  }

  internal static bool IsTimescaleValid(float scale)
  {
    return (double) scale > 0.0 && float.IsNormal(scale) && float.IsFinite(scale);
  }
}
