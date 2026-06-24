using System;
using System.Threading;
using System.Threading.Tasks;
using Robust.Shared.Log;
using Robust.Shared.Timing;

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

	protected Job(double maxTime, CancellationToken cancellation = default(CancellationToken))
		: this(maxTime, (IStopwatch)new Stopwatch(), cancellation)
	{
	}

	protected Job(double maxTime, IStopwatch stopwatch, CancellationToken cancellation = default(CancellationToken))
	{
		MaxTime = maxTime;
		StopWatch = stopwatch;
		Cancellation = cancellation;
		_taskTcs = new TaskCompletionSource<T>();
		AsTask = _taskTcs.Task;
	}

	protected Task SuspendNow()
	{
		_resume = new TaskCompletionSource<object>();
		Status = JobStatus.Paused;
		DebugTime += StopWatch.Elapsed.TotalSeconds;
		return _resume.Task;
	}

	protected ValueTask SuspendIfOutOfTime()
	{
		if (StopWatch.Elapsed.TotalSeconds <= MaxTime || MaxTime == 0.0)
		{
			return default(ValueTask);
		}
		return new ValueTask(SuspendNow());
	}

	protected async Task<TTask> WaitAsyncTask<TTask>(Task<TTask> task)
	{
		Status = JobStatus.Waiting;
		DebugTime += StopWatch.Elapsed.TotalSeconds;
		TTask result = await task;
		Status = JobStatus.Paused;
		_resume = new TaskCompletionSource<object>();
		await _resume.Task;
		return result;
	}

	protected async Task WaitAsyncTask(Task task)
	{
		Status = JobStatus.Waiting;
		DebugTime += StopWatch.Elapsed.TotalSeconds;
		await task;
		_resume = new TaskCompletionSource<object>();
		Status = JobStatus.Paused;
		await _resume.Task;
	}

	public void Run()
	{
		StopWatch.Restart();
		if (_workInProgress == null)
		{
			_workInProgress = ProcessWrap();
		}
		if (Status != JobStatus.Finished)
		{
			TaskCompletionSource<object> resume = _resume;
			_resume = null;
			Status = JobStatus.Running;
			if (Cancellation.IsCancellationRequested)
			{
				resume?.TrySetCanceled();
			}
			else
			{
				resume?.SetResult(null);
			}
			if (Status != JobStatus.Finished)
			{
				_ = Status;
				_ = 3;
			}
		}
	}

	protected abstract Task<T?> Process();

	private async Task ProcessWrap()
	{
		_ = 1;
		try
		{
			Cancellation.ThrowIfCancellationRequested();
			await SuspendNow();
			Result = await Process();
			_taskTcs.TrySetResult(Result);
		}
		catch (OperationCanceledException)
		{
			_taskTcs.TrySetCanceled();
		}
		catch (Exception ex2)
		{
			_sawmill.Error("Job failed on exception:\n{0}", ex2);
			Exception = ex2;
			_taskTcs.TrySetException(ex2);
		}
		finally
		{
			if (Status != JobStatus.Waiting)
			{
				DebugTime += StopWatch.Elapsed.TotalSeconds;
			}
			Status = JobStatus.Finished;
		}
	}
}
