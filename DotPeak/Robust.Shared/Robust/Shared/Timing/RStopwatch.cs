// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Timing.RStopwatch
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable disable
namespace Robust.Shared.Timing;

public struct RStopwatch
{
  private long _curTicks;
  private static readonly double TicksToTimeTicks = 10000000.0 / (double) System.Diagnostics.Stopwatch.Frequency;

  public bool IsRunning { get; private set; }

  public static RStopwatch StartNew()
  {
    RStopwatch rstopwatch = new RStopwatch();
    rstopwatch.Start();
    return rstopwatch;
  }

  public void Start()
  {
    if (this.IsRunning)
      return;
    this._curTicks = System.Diagnostics.Stopwatch.GetTimestamp() - this._curTicks;
    this.IsRunning = true;
  }

  public void Restart()
  {
    this.IsRunning = true;
    this._curTicks = System.Diagnostics.Stopwatch.GetTimestamp();
  }

  public void Stop()
  {
    if (!this.IsRunning)
      return;
    this._curTicks = this.ElapsedTicks;
    this.IsRunning = false;
  }

  public unsafe void Reset() => *(RStopwatch*) ref this = new RStopwatch();

  public readonly long ElapsedTicks
  {
    get => !this.IsRunning ? this._curTicks : System.Diagnostics.Stopwatch.GetTimestamp() - this._curTicks;
  }

  public readonly TimeSpan Elapsed => new TimeSpan(this.ElapsedTimeTicks());

  private readonly long ElapsedTimeTicks()
  {
    return (long) (RStopwatch.TicksToTimeTicks * (double) this.ElapsedTicks);
  }
}
