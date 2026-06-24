// Decompiled with JetBrains decompiler
// Type: Robust.Shared.CPUJob.JobQueues.Queues.JobQueue
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.CPUJob.JobQueues.Queues;

[Virtual]
public class JobQueue
{
  private readonly IStopwatch _stopwatch;
  private readonly Queue<IJob> _pendingQueue = new Queue<IJob>();
  private readonly List<IJob> _waitingJobs = new List<IJob>();

  public JobQueue(double maxTime)
    : this((IStopwatch) new Stopwatch())
  {
    this.MaxTime = maxTime;
  }

  public JobQueue()
    : this((IStopwatch) new Stopwatch())
  {
  }

  public JobQueue(IStopwatch stopwatch) => this._stopwatch = stopwatch;

  public virtual double MaxTime { get; } = 0.002;

  public void EnqueueJob(IJob job) => this._pendingQueue.Enqueue(job);

  public void Process()
  {
    foreach (IJob waitingJob in this._waitingJobs)
    {
      if (waitingJob.Status != JobStatus.Waiting)
        this._pendingQueue.Enqueue(waitingJob);
    }
    this._waitingJobs.RemoveAll((Predicate<IJob>) (p => p.Status != JobStatus.Waiting));
    this._stopwatch.Restart();
    IJob result;
    while (this._stopwatch.Elapsed.TotalSeconds < this.MaxTime && this._pendingQueue.TryDequeue(out result))
    {
      result.Run();
      switch (result.Status)
      {
        case JobStatus.Waiting:
          this._waitingJobs.Add(result);
          continue;
        case JobStatus.Finished:
          continue;
        default:
          this._pendingQueue.Enqueue(result);
          continue;
      }
    }
  }
}
