// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Timing.TimerManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Exceptions;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable enable
namespace Robust.Shared.Timing;

internal sealed class TimerManager : ITimerManager
{
  [Dependency]
  private readonly IRuntimeLog _runtimeLog;
  private readonly List<(Timer, CancellationToken)> _timers = new List<(Timer, CancellationToken)>();

  public void AddTimer(Timer timer, CancellationToken cancellationToken = default (CancellationToken))
  {
    this._timers.Add((timer, cancellationToken));
  }

  public void UpdateTimers(FrameEventArgs frameEventArgs)
  {
    for (int index = 0; index < this._timers.Count; ++index)
    {
      (Timer, CancellationToken) timer1 = this._timers[index];
      Timer timer2 = timer1.Item1;
      if (!timer1.Item2.IsCancellationRequested)
        timer2.Update(frameEventArgs.DeltaSeconds, this._runtimeLog);
    }
    this._timers.RemoveAll((Predicate<(Timer, CancellationToken)>) (timer => !timer.Item1.IsActive || timer.Item2.IsCancellationRequested));
  }
}
