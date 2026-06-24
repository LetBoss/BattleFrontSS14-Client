// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Timing.Timer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Exceptions;
using Robust.Shared.IoC;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Timing;

public sealed class Timer
{
  private float _timeCounter;

  public int Time { get; }

  public bool IsRepeating { get; }

  public bool IsActive { get; private set; } = true;

  public Action OnFired { get; }

  public Timer(int milliseconds, bool isRepeating, Action onFired)
  {
    this._timeCounter = (float) (this.Time = milliseconds);
    this.IsRepeating = isRepeating;
    this.OnFired = onFired;
  }

  public void Update(float frameTime, IRuntimeLog runtimeLog)
  {
    if (!this.IsActive)
      return;
    this._timeCounter -= frameTime * 1000f;
    if ((double) this._timeCounter > 0.0)
      return;
    try
    {
      this.OnFired();
    }
    catch (Exception ex)
    {
      runtimeLog.LogException(ex, "Timer Callback");
    }
    if (this.IsRepeating)
      this._timeCounter += (float) this.Time;
    else
      this.IsActive = false;
  }

  public static Task Delay(int milliseconds, CancellationToken cancellationToken = default (CancellationToken))
  {
    TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
    Timer.Spawn(milliseconds, (Action) (() => tcs.SetResult((object) null)), cancellationToken);
    return (Task) tcs.Task;
  }

  public static Task Delay(TimeSpan duration, CancellationToken cancellationToken = default (CancellationToken))
  {
    return Timer.Delay((int) duration.TotalMilliseconds, cancellationToken);
  }

  public static void Spawn(int milliseconds, Action onFired, CancellationToken cancellationToken = default (CancellationToken))
  {
    Timer timer = new Timer(milliseconds, false, onFired);
    IoCManager.Resolve<ITimerManager>().AddTimer(timer, cancellationToken);
  }

  public static void Spawn(TimeSpan duration, Action onFired, CancellationToken cancellationToken = default (CancellationToken))
  {
    Timer.Spawn((int) duration.TotalMilliseconds, onFired, cancellationToken);
  }

  public static void SpawnRepeating(
    int milliseconds,
    Action onFired,
    CancellationToken cancellationToken)
  {
    Timer timer = new Timer(milliseconds, true, onFired);
    IoCManager.Resolve<ITimerManager>().AddTimer(timer, cancellationToken);
  }

  public static void SpawnRepeating(
    TimeSpan duration,
    Action onFired,
    CancellationToken cancellationToken)
  {
    Timer.SpawnRepeating((int) duration.TotalMilliseconds, onFired, cancellationToken);
  }
}
