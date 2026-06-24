// Decompiled with JetBrains decompiler
// Type: Robust.Shared.CPUJob.JobQueues.Job`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Log;
using Robust.Shared.Timing;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.CPUJob.JobQueues;

public abstract class Job<T> : IJob
{
  private readonly ISawmill _sawmill = Logger.GetSawmill("job");
  public double MaxTime;
  protected readonly IStopwatch StopWatch;
  private readonly TaskCompletionSource<T?> _taskTcs;
  private TaskCompletionSource<object?>? _resume;
  private Task? _workInProgress;

  public JobStatus Status { get; private set; }

  public Task<T?> AsTask { get; }

  public T? Result { get; private set; }

  public Exception? Exception { get; private set; }

  protected CancellationToken Cancellation { get; }

  public double DebugTime { get; private set; }

  protected Job(double maxTime, CancellationToken cancellation = default (CancellationToken))
    : this(maxTime, (IStopwatch) new Stopwatch(), cancellation)
  {
  }

  protected Job(double maxTime, IStopwatch stopwatch, CancellationToken cancellation = default (CancellationToken))
  {
    this.MaxTime = maxTime;
    this.StopWatch = stopwatch;
    this.Cancellation = cancellation;
    this._taskTcs = new TaskCompletionSource<T>();
    this.AsTask = this._taskTcs.Task;
  }

  protected Task SuspendNow()
  {
    this._resume = new TaskCompletionSource<object>();
    this.Status = JobStatus.Paused;
    this.DebugTime += this.StopWatch.Elapsed.TotalSeconds;
    return (Task) this._resume.Task;
  }

  protected ValueTask SuspendIfOutOfTime()
  {
    return this.StopWatch.Elapsed.TotalSeconds <= this.MaxTime || this.MaxTime == 0.0 ? new ValueTask() : new ValueTask(this.SuspendNow());
  }

  protected async Task<TTask> WaitAsyncTask<TTask>(Task<TTask> task)
  {
    this.Status = JobStatus.Waiting;
    this.DebugTime += this.StopWatch.Elapsed.TotalSeconds;
    TTask result = await task;
    this.Status = JobStatus.Paused;
    this._resume = new TaskCompletionSource<object>();
    object task1 = await this._resume.Task;
    TTask task2 = result;
    result = default (TTask);
    return task2;
  }

  protected async Task WaitAsyncTask(Task task)
  {
    this.Status = JobStatus.Waiting;
    this.DebugTime += this.StopWatch.Elapsed.TotalSeconds;
    await task;
    this._resume = new TaskCompletionSource<object>();
    this.Status = JobStatus.Paused;
    object task1 = await this._resume.Task;
  }

  public void Run()
  {
    this.StopWatch.Restart();
    if (this._workInProgress == null)
      this._workInProgress = this.ProcessWrap();
    if (this.Status == JobStatus.Finished)
      return;
    TaskCompletionSource<object> resume = this._resume;
    this._resume = (TaskCompletionSource<object>) null;
    this.Status = JobStatus.Running;
    if (this.Cancellation.IsCancellationRequested)
      resume?.TrySetCanceled();
    else
      resume?.SetResult((object) null);
    if (this.Status == JobStatus.Finished)
      return;
    int status = (int) this.Status;
  }

  protected abstract Task<T?> Process();

  private async Task ProcessWrap()
  {
    try
    {
      this.Cancellation.ThrowIfCancellationRequested();
      await this.SuspendNow();
      this.Result = await this.Process();
      this._taskTcs.TrySetResult(this.Result);
    }
    catch (OperationCanceledException ex)
    {
      this._taskTcs.TrySetCanceled();
    }
    catch (Exception ex)
    {
      this._sawmill.Error("Job failed on exception:\n{0}", (object) ex);
      this.Exception = ex;
      this._taskTcs.TrySetException(ex);
    }
    finally
    {
      if (this.Status != JobStatus.Waiting)
        this.DebugTime += this.StopWatch.Elapsed.TotalSeconds;
      this.Status = JobStatus.Finished;
    }
  }
}
