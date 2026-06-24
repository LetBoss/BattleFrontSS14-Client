// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Timing.GameLoop
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Prometheus;
using Robust.Shared.Exceptions;
using Robust.Shared.Log;
using Robust.Shared.Profiling;
using System;
using System.Threading;

#nullable enable
namespace Robust.Shared.Timing;

internal sealed class GameLoop : IGameLoop
{
  private static readonly TimeSpan DelayTime = TimeSpan.FromMilliseconds(1L);
  public const string ProfTextStartFrame = "Start Frame";
  private static readonly Histogram _frameTimeHistogram = Metrics.CreateHistogram("robust_game_loop_frametime", "Histogram of frametimes in ms", new HistogramConfiguration()
  {
    Buckets = Histogram.ExponentialBuckets(0.001, 1.5, 10)
  });
  private readonly IGameTiming _timing;
  private TimeSpan _lastKeepUp;
  private readonly IRuntimeLog _runtimeLog;
  private readonly ProfManager _prof;
  private readonly ISawmill _sawmill;
  private readonly PrecisionSleep _precisionSleep;
  private int _tickExceptions;
  private const int MaxSoftLockExceptions = 10;

  public event EventHandler<FrameEventArgs>? Input;

  public event EventHandler<FrameEventArgs>? Tick;

  public event EventHandler<FrameEventArgs>? Update;

  public event EventHandler<FrameEventArgs>? Render;

  public bool SingleStep { get; set; }

  public bool Running { get; set; }

  public int MaxQueuedTicks { get; set; } = 5;

  public TimeSpan LimitMinFrameTime { get; set; }

  public bool DetectSoftLock { get; set; }

  public bool EnableMetrics { get; set; }

  public SleepMode SleepMode { get; set; }

  public GameLoop(
    IGameTiming timing,
    IRuntimeLog runtimeLog,
    ProfManager prof,
    ISawmill sawmill,
    GameLoopOptions options)
  {
    this._timing = timing;
    this._runtimeLog = runtimeLog;
    this._prof = prof;
    this._sawmill = sawmill;
    this._precisionSleep = options.Precise ? PrecisionSleep.Create() : (PrecisionSleep) new PrecisionSleepUniversal();
  }

  public void Run()
  {
    if (this._timing.TickRate <= (ushort) 0)
      throw new InvalidOperationException("TickRate must be greater than 0.");
    this.Running = true;
    while (this.Running)
    {
      long start = this._prof.WriteValue("Start Frame", ProfData.Int64((long) this._timing.CurFrame));
      long startIndex = this._prof.WriteGroupStart();
      ProfSampler sampler = ProfSampler.StartNew();
      int num1 = GC.CollectionCount(0);
      int num2 = GC.CollectionCount(1);
      int num3 = GC.CollectionCount(2);
      this._timing.StartFrame();
      TimeSpan timeSpan1 = TimeSpan.FromTicks(this._timing.TickPeriod.Ticks * (long) this.MaxQueuedTicks);
      TimeSpan timeSpan2 = this._timing.RealTime - this._timing.LastTick;
      if (timeSpan2 > timeSpan1)
      {
        timeSpan2 = timeSpan1;
        this._timing.LastTick = this._timing.RealTime - timeSpan1;
        if ((this._timing.RealTime - this._lastKeepUp).TotalSeconds >= 15.0)
        {
          GameLoopEventSource.Log.CannotKeepUp();
          this._sawmill.Warning("MainLoop: Cannot keep up!");
          this._lastKeepUp = this._timing.RealTime;
        }
      }
      FrameEventArgs e1 = new FrameEventArgs((float) this._timing.RealFrameTime.TotalSeconds);
      GameLoopEventSource.Log.InputStart();
      try
      {
        using (this._prof.Group("Input"))
        {
          EventHandler<FrameEventArgs> input = this.Input;
          if (input != null)
            input((object) this, e1);
        }
      }
      catch (Exception ex)
      {
        this._runtimeLog.LogException(ex, "GameLoop Input");
      }
      GameLoopEventSource.Log.InputStop();
      this._timing.InSimulation = true;
      TimeSpan timeSpan3 = this._timing.CalcAdjustedTickPeriod();
      using (this._prof.Group("Ticks"))
      {
        int int32 = 0;
        while (timeSpan2 >= timeSpan3)
        {
          timeSpan2 -= timeSpan3;
          this._timing.LastTick += timeSpan3;
          if (!this._timing.Paused)
          {
            this._timing.TickRemainder = timeSpan2 / (double) this._timing.TimeScale;
            ++int32;
            FrameEventArgs e2 = new FrameEventArgs((float) this._timing.FrameTime.TotalSeconds);
            bool flag = false;
            try
            {
              GameLoopEventSource.Log.TickStart(this._timing.CurTick.Value);
              using (this._prof.Group("Tick"))
              {
                this._prof.WriteValue("Tick", ProfData.Int64((long) this._timing.CurTick.Value));
                if (this.EnableMetrics)
                {
                  using (TimerExtensions.NewTimer((IObserver) GameLoop._frameTimeHistogram))
                  {
                    EventHandler<FrameEventArgs> tick = this.Tick;
                    if (tick != null)
                      tick((object) this, e2);
                  }
                }
                else
                {
                  EventHandler<FrameEventArgs> tick = this.Tick;
                  if (tick != null)
                    tick((object) this, e2);
                }
                GameLoopEventSource.Log.TickStop(this._timing.CurTick.Value);
              }
            }
            catch (Exception ex)
            {
              flag = true;
              this._runtimeLog.LogException(ex, "GameLoop Tick");
              ++this._tickExceptions;
              if (this._tickExceptions > 10)
              {
                if (this.DetectSoftLock)
                {
                  this._sawmill.Fatal("MainLoop: 10 consecutive exceptions inside GameLoop Tick, shutting down!");
                  this.Running = false;
                }
              }
            }
            if (!flag)
              this._tickExceptions = 0;
            this._timing.CurTick = new GameTick(this._timing.CurTick.Value + 1U);
            timeSpan3 = this._timing.CalcAdjustedTickPeriod();
            if (this.SingleStep)
              this._timing.Paused = true;
          }
        }
        this._prof.WriteValue("Tick count", ProfData.Int32(int32));
      }
      if (!this._timing.Paused)
        this._timing.TickRemainder = timeSpan2 / (double) this._timing.TimeScale;
      this._timing.InSimulation = false;
      GameLoopEventSource.Log.UpdateStart();
      try
      {
        using (this._prof.Group("Update"))
        {
          EventHandler<FrameEventArgs> update = this.Update;
          if (update != null)
            update((object) this, e1);
        }
      }
      catch (Exception ex)
      {
        this._runtimeLog.LogException(ex, "GameLoop Update");
      }
      GameLoopEventSource.Log.UpdateStop();
      try
      {
        using (this._prof.Group("Render"))
        {
          EventHandler<FrameEventArgs> render = this.Render;
          if (render != null)
            render((object) this, e1);
        }
      }
      catch (Exception ex)
      {
        this._runtimeLog.LogException(ex, "GameLoop Render");
      }
      using (this._prof.Group("GC Overview"))
      {
        this._prof.WriteValue("Gen 0 Count", ProfData.Int32(GC.CollectionCount(0) - num1));
        this._prof.WriteValue("Gen 1 Count", ProfData.Int32(GC.CollectionCount(1) - num2));
        this._prof.WriteValue("Gen 2 Count", ProfData.Int32(GC.CollectionCount(2) - num3));
      }
      this._prof.WriteGroupEnd(startIndex, "Frame", in sampler);
      this._prof.MarkIndex(start, ProfIndexType.Frame);
      GameLoopEventSource.Log.SleepStart();
      switch (this.SleepMode)
      {
        case SleepMode.Yield:
          Thread.Sleep(0);
          break;
        case SleepMode.Delay:
          TimeSpan time1 = this._timing.LastTick + this._timing.TickPeriod - this._timing.RealTime;
          if (time1 > GameLoop.DelayTime)
            time1 = GameLoop.DelayTime;
          if (time1.Ticks > 0L)
          {
            this._precisionSleep.Sleep(time1);
            break;
          }
          break;
        case SleepMode.Limit:
          TimeSpan timeSpan4 = this._timing.RealFrameTime - this.LimitMinFrameTime;
          if (timeSpan4.Ticks < 0L)
            timeSpan4 = TimeSpan.Zero;
          TimeSpan time2 = this.LimitMinFrameTime - (this._timing.RealTime - this._timing.FrameStartTime) - timeSpan4;
          if (time2.Ticks > 0L)
          {
            RStopwatch.StartNew();
            this._precisionSleep.Sleep(time2);
            break;
          }
          break;
      }
      GameLoopEventSource.Log.SleepStop();
    }
  }
}
