// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Profiling.ProfSampler
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Timing;
using System;

#nullable disable
namespace Robust.Shared.Profiling;

public struct ProfSampler
{
  public RStopwatch Stopwatch;
  private long _alloc;

  public static ProfSampler StartNew()
  {
    ProfSampler profSampler = new ProfSampler();
    profSampler.Start();
    return profSampler;
  }

  public void Start()
  {
    if (this.Stopwatch.IsRunning)
      return;
    this.Stopwatch.Start();
    this._alloc = GC.GetAllocatedBytesForCurrentThread() - this._alloc;
  }

  public void Restart()
  {
    this.Stopwatch.Restart();
    this._alloc = GC.GetAllocatedBytesForCurrentThread();
  }

  public void Stop()
  {
    if (!this.Stopwatch.IsRunning)
      return;
    this._alloc = this.ElapsedAlloc;
    this.Stopwatch.Stop();
  }

  public unsafe void Reset() => *(ProfSampler*) ref this = new ProfSampler();

  public readonly long ElapsedAlloc
  {
    get
    {
      return !this.Stopwatch.IsRunning ? this._alloc : GC.GetAllocatedBytesForCurrentThread() - this._alloc;
    }
  }

  public readonly TimeSpan Elapsed => this.Stopwatch.Elapsed;
}
