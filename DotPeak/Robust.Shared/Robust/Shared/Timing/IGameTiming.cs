// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Timing.IGameTiming
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Robust.Shared.Timing;

[NotContentImplementable]
public interface IGameTiming
{
  bool InSimulation { get; set; }

  bool Paused { get; set; }

  TimeSpan CurTime { get; }

  TimeSpan RealTime { get; }

  TimeSpan FrameStartTime { get; }

  TimeSpan ServerTime { get; }

  TimeSpan FrameTime { get; }

  TimeSpan RealFrameTime { get; }

  TimeSpan RealFrameTimeAvg { get; }

  TimeSpan RealFrameTimeStdDev { get; }

  uint CurFrame { get; set; }

  double FramesPerSecondAvg { get; }

  GameTick CurTick { get; set; }

  TimeSpan LastTick { get; set; }

  ushort TickRate { get; set; }

  float TimeScale { get; set; }

  (TimeSpan, GameTick) TimeBase { get; set; }

  TimeSpan TickPeriod { get; }

  TimeSpan TickRemainder { get; set; }

  TimeSpan TickRemainderRealtime { get; }

  TimeSpan CalcAdjustedTickPeriod();

  ushort TickFraction
  {
    get
    {
      if (this.InSimulation)
        return ushort.MaxValue;
      TimeSpan timeSpan = this.TickRemainder;
      double num = (double) ushort.MaxValue * timeSpan.TotalSeconds;
      timeSpan = this.TickPeriod;
      double totalSeconds = timeSpan.TotalSeconds;
      return (ushort) (num / totalSeconds);
    }
  }

  float TickTimingAdjustment { get; set; }

  void StartFrame();

  bool IsFirstTimePredicted { get; }

  bool InPrediction { get; }

  bool ApplyingState { get; }

  string TickStamp
  {
    get
    {
      return $"{this.CurTick}, predFirst: {this.IsFirstTimePredicted}, tickRem: {this.TickRemainder.TotalSeconds}, sim: {this.InSimulation}";
    }
  }

  static string TickStampStatic => IoCManager.Resolve<IGameTiming>().TickStamp;

  void ResetSimTime();

  void ResetSimTime((TimeSpan, GameTick) timeBase);

  void SetTickRateAt(ushort tickRate, GameTick atTick);

  TimeSpan RealLocalToServer(TimeSpan local);

  TimeSpan RealServerToLocal(TimeSpan server);
}
