// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Threading.ParallelManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Microsoft.Extensions.ObjectPool;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using System;
using System.Threading;

#nullable enable
namespace Robust.Shared.Threading;

internal sealed class ParallelManager : IParallelManagerInternal, IParallelManager
{
  [Dependency]
  private readonly IConfigurationManager _cfg;
  [Dependency]
  private readonly ILogManager _logs;
  public static readonly ManualResetEventSlim DummyResetEvent = new ManualResetEventSlim(true);
  private ISawmill _sawmill;
  private readonly Microsoft.Extensions.ObjectPool.ObjectPool<ParallelManager.InternalJob> _jobPool = (Microsoft.Extensions.ObjectPool.ObjectPool<ParallelManager.InternalJob>) new DefaultObjectPool<ParallelManager.InternalJob>((IPooledObjectPolicy<ParallelManager.InternalJob>) new DefaultPooledObjectPolicy<ParallelManager.InternalJob>(), 1024 /*0x0400*/);
  private readonly Microsoft.Extensions.ObjectPool.ObjectPool<ParallelManager.InternalParallelRangeJob> _parallelPool = (Microsoft.Extensions.ObjectPool.ObjectPool<ParallelManager.InternalParallelRangeJob>) new DefaultObjectPool<ParallelManager.InternalParallelRangeJob>((IPooledObjectPolicy<ParallelManager.InternalParallelRangeJob>) new DefaultPooledObjectPolicy<ParallelManager.InternalParallelRangeJob>(), 1024 /*0x0400*/);
  private readonly Microsoft.Extensions.ObjectPool.ObjectPool<ParallelManager.ParallelTracker> _trackerPool = (Microsoft.Extensions.ObjectPool.ObjectPool<ParallelManager.ParallelTracker>) new DefaultObjectPool<ParallelManager.ParallelTracker>((IPooledObjectPolicy<ParallelManager.ParallelTracker>) new DefaultPooledObjectPolicy<ParallelManager.ParallelTracker>(), 1024 /*0x0400*/);

  public event Action? ParallelCountChanged;

  public int ParallelProcessCount { get; private set; }

  public void Initialize()
  {
    this._sawmill = this._logs.GetSawmill("parallel");
    this._cfg.OnValueChanged<int>(CVars.ThreadParallelCount, new Action<int>(this.UpdateCVar), true);
  }

  public void AddAndInvokeParallelCountChanged(Action changed)
  {
    this.ParallelCountChanged += changed;
    changed();
  }

  private ParallelManager.InternalJob GetJob(IRobustJob job)
  {
    ParallelManager.InternalJob job1 = this._jobPool.Get();
    job1.Event.Reset();
    job1.Set(this._sawmill, job, this._jobPool);
    return job1;
  }

  private ParallelManager.InternalParallelRangeJob GetParallelJob(
    IParallelRangeRobustJob job,
    int start,
    int end,
    ParallelManager.ParallelTracker tracker)
  {
    ParallelManager.InternalParallelRangeJob parallelJob = this._parallelPool.Get();
    parallelJob.Set(this._sawmill, job, start, end, tracker, this._parallelPool);
    return parallelJob;
  }

  private void UpdateCVar(int value)
  {
    int parallelProcessCount1 = this.ParallelProcessCount;
    int workerThreads;
    int completionPortThreads;
    ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
    this.ParallelProcessCount = value == 0 ? workerThreads : value;
    int parallelProcessCount2 = this.ParallelProcessCount;
    if (parallelProcessCount1 == parallelProcessCount2)
      return;
    Action parallelCountChanged = this.ParallelCountChanged;
    if (parallelCountChanged != null)
      parallelCountChanged();
    ThreadPool.SetMaxThreads(this.ParallelProcessCount, completionPortThreads);
  }

  public WaitHandle Process(IRobustJob job)
  {
    ParallelManager.InternalJob job1 = this.GetJob(job);
    ThreadPool.UnsafeQueueUserWorkItem((IThreadPoolWorkItem) job1, true);
    return job1.Event.WaitHandle;
  }

  public void ProcessNow(IRobustJob job) => job.Execute();

  public void ProcessNow(IParallelRobustJob jobs, int amount)
  {
    this.ProcessNow((IParallelRangeRobustJob) jobs, amount);
  }

  public void ProcessNow(IParallelBulkRobustJob jobs, int amount)
  {
    this.ProcessNow((IParallelRangeRobustJob) jobs, amount);
  }

  public void ProcessSerialNow(IParallelRobustJob jobs, int amount)
  {
    this.ProcessSerialNow((IParallelRangeRobustJob) jobs, amount);
  }

  public void ProcessSerialNow(IParallelBulkRobustJob jobs, int amount)
  {
    this.ProcessSerialNow((IParallelRangeRobustJob) jobs, amount);
  }

  public WaitHandle Process(IParallelRobustJob jobs, int amount)
  {
    return this.Process((IParallelRangeRobustJob) jobs, amount);
  }

  public WaitHandle Process(IParallelBulkRobustJob jobs, int amount)
  {
    return this.Process((IParallelRangeRobustJob) jobs, amount);
  }

  public void ProcessNow(IParallelRangeRobustJob job, int amount)
  {
    if ((double) amount / (double) job.BatchSize <= (double) job.MinimumBatchParallel)
    {
      this.ProcessSerialNow(job, amount);
    }
    else
    {
      ParallelManager.ParallelTracker parallelTracker = this.InternalProcess(job, amount);
      parallelTracker.Event.WaitHandle.WaitOne();
      this._trackerPool.Return(parallelTracker);
    }
  }

  public void ProcessSerialNow(IParallelRangeRobustJob jobs, int amount)
  {
    if (amount <= 0)
      return;
    jobs.ExecuteRange(0, amount);
  }

  public WaitHandle Process(IParallelRangeRobustJob job, int amount)
  {
    return this.InternalProcess(job, amount).Event.WaitHandle;
  }

  private ParallelManager.ParallelTracker InternalProcess(IParallelRangeRobustJob job, int amount)
  {
    int num = (int) MathF.Ceiling((float) amount / (float) job.BatchSize);
    int batchSize = job.BatchSize;
    ParallelManager.ParallelTracker tracker = this._trackerPool.Get();
    tracker.Event.Reset();
    if (amount <= 0)
    {
      tracker.Event.Set();
      return tracker;
    }
    tracker.PendingTasks = num;
    for (int index = 0; index < num; ++index)
    {
      int start = index * batchSize;
      int end = Math.Min(start + batchSize, amount);
      ThreadPool.UnsafeQueueUserWorkItem((IThreadPoolWorkItem) this.GetParallelJob(job, start, end, tracker), true);
    }
    return tracker;
  }

  private sealed class InternalJob : IRobustJob, IThreadPoolWorkItem
  {
    private ISawmill _sawmill;
    private IRobustJob _robust;
    public readonly ManualResetEventSlim Event = new ManualResetEventSlim();
    private Microsoft.Extensions.ObjectPool.ObjectPool<ParallelManager.InternalJob> _parentPool;

    public void Set(
      ISawmill sawmill,
      IRobustJob job,
      Microsoft.Extensions.ObjectPool.ObjectPool<ParallelManager.InternalJob> parentPool)
    {
      this._sawmill = sawmill;
      this._robust = job;
      this._parentPool = parentPool;
    }

    public void Execute()
    {
      try
      {
        this._robust.Execute();
      }
      catch (Exception ex)
      {
        this._sawmill.Error($"Exception in ParallelManager: {ex}");
      }
      finally
      {
        this.Event.Set();
        this._parentPool.Return(this);
      }
    }
  }

  private sealed class InternalParallelRangeJob : IRobustJob, IThreadPoolWorkItem
  {
    private IParallelRangeRobustJob _robust;
    private int _start;
    private int _end;
    private ISawmill _sawmill;
    private ParallelManager.ParallelTracker _tracker;
    private Microsoft.Extensions.ObjectPool.ObjectPool<ParallelManager.InternalParallelRangeJob> _parentPool;

    public void Set(
      ISawmill sawmill,
      IParallelRangeRobustJob robust,
      int start,
      int end,
      ParallelManager.ParallelTracker tracker,
      Microsoft.Extensions.ObjectPool.ObjectPool<ParallelManager.InternalParallelRangeJob> parentPool)
    {
      this._sawmill = sawmill;
      this._robust = robust;
      this._start = start;
      this._end = end;
      this._tracker = tracker;
      this._parentPool = parentPool;
    }

    public void Execute()
    {
      try
      {
        this._robust.ExecuteRange(this._start, this._end);
      }
      catch (Exception ex)
      {
        this._sawmill.Error($"Exception in ParallelManager: {ex}");
      }
      finally
      {
        this._tracker.Set();
        this._parentPool.Return(this);
      }
    }
  }

  private sealed class ParallelTracker
  {
    public readonly ManualResetEventSlim Event = new ManualResetEventSlim();
    public int PendingTasks;

    public void Set()
    {
      if (Interlocked.Decrement(ref this.PendingTasks) > 0)
        return;
      this.Event.Set();
    }
  }
}
