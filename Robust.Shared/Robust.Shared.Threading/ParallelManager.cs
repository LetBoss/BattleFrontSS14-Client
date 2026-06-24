using System;
using System.Threading;
using Microsoft.Extensions.ObjectPool;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Log;

namespace Robust.Shared.Threading;

internal sealed class ParallelManager : IParallelManagerInternal, IParallelManager
{
	private sealed class InternalJob : IRobustJob, IThreadPoolWorkItem
	{
		private ISawmill _sawmill;

		private IRobustJob _robust;

		public readonly ManualResetEventSlim Event = new ManualResetEventSlim();

		private ObjectPool<InternalJob> _parentPool;

		public void Set(ISawmill sawmill, IRobustJob job, ObjectPool<InternalJob> parentPool)
		{
			_sawmill = sawmill;
			_robust = job;
			_parentPool = parentPool;
		}

		public void Execute()
		{
			try
			{
				_robust.Execute();
			}
			catch (Exception value)
			{
				_sawmill.Error($"Exception in ParallelManager: {value}");
			}
			finally
			{
				Event.Set();
				_parentPool.Return(this);
			}
		}
	}

	private sealed class InternalParallelRangeJob : IRobustJob, IThreadPoolWorkItem
	{
		private IParallelRangeRobustJob _robust;

		private int _start;

		private int _end;

		private ISawmill _sawmill;

		private ParallelTracker _tracker;

		private ObjectPool<InternalParallelRangeJob> _parentPool;

		public void Set(ISawmill sawmill, IParallelRangeRobustJob robust, int start, int end, ParallelTracker tracker, ObjectPool<InternalParallelRangeJob> parentPool)
		{
			_sawmill = sawmill;
			_robust = robust;
			_start = start;
			_end = end;
			_tracker = tracker;
			_parentPool = parentPool;
		}

		public void Execute()
		{
			try
			{
				_robust.ExecuteRange(_start, _end);
			}
			catch (Exception value)
			{
				_sawmill.Error($"Exception in ParallelManager: {value}");
			}
			finally
			{
				_tracker.Set();
				_parentPool.Return(this);
			}
		}
	}

	private sealed class ParallelTracker
	{
		public readonly ManualResetEventSlim Event = new ManualResetEventSlim();

		public int PendingTasks;

		public void Set()
		{
			if (Interlocked.Decrement(ref PendingTasks) <= 0)
			{
				Event.Set();
			}
		}
	}

	[Dependency]
	private readonly IConfigurationManager _cfg;

	[Dependency]
	private readonly ILogManager _logs;

	public static readonly ManualResetEventSlim DummyResetEvent = new ManualResetEventSlim(initialState: true);

	private ISawmill _sawmill;

	private readonly ObjectPool<InternalJob> _jobPool = new DefaultObjectPool<InternalJob>(new DefaultPooledObjectPolicy<InternalJob>(), 1024);

	private readonly ObjectPool<InternalParallelRangeJob> _parallelPool = new DefaultObjectPool<InternalParallelRangeJob>(new DefaultPooledObjectPolicy<InternalParallelRangeJob>(), 1024);

	private readonly ObjectPool<ParallelTracker> _trackerPool = new DefaultObjectPool<ParallelTracker>(new DefaultPooledObjectPolicy<ParallelTracker>(), 1024);

	public int ParallelProcessCount { get; private set; }

	public event Action? ParallelCountChanged;

	public void Initialize()
	{
		_sawmill = _logs.GetSawmill("parallel");
		_cfg.OnValueChanged(CVars.ThreadParallelCount, UpdateCVar, invokeImmediately: true);
	}

	public void AddAndInvokeParallelCountChanged(Action changed)
	{
		ParallelCountChanged += changed;
		changed();
	}

	private InternalJob GetJob(IRobustJob job)
	{
		InternalJob internalJob = _jobPool.Get();
		internalJob.Event.Reset();
		internalJob.Set(_sawmill, job, _jobPool);
		return internalJob;
	}

	private InternalParallelRangeJob GetParallelJob(IParallelRangeRobustJob job, int start, int end, ParallelTracker tracker)
	{
		InternalParallelRangeJob internalParallelRangeJob = _parallelPool.Get();
		internalParallelRangeJob.Set(_sawmill, job, start, end, tracker, _parallelPool);
		return internalParallelRangeJob;
	}

	private void UpdateCVar(int value)
	{
		int parallelProcessCount = ParallelProcessCount;
		ThreadPool.GetAvailableThreads(out var workerThreads, out var completionPortThreads);
		ParallelProcessCount = ((value == 0) ? workerThreads : value);
		if (parallelProcessCount != ParallelProcessCount)
		{
			this.ParallelCountChanged?.Invoke();
			ThreadPool.SetMaxThreads(ParallelProcessCount, completionPortThreads);
		}
	}

	public WaitHandle Process(IRobustJob job)
	{
		InternalJob job2 = GetJob(job);
		ThreadPool.UnsafeQueueUserWorkItem(job2, preferLocal: true);
		return job2.Event.WaitHandle;
	}

	public void ProcessNow(IRobustJob job)
	{
		job.Execute();
	}

	public void ProcessNow(IParallelRobustJob jobs, int amount)
	{
		ProcessNow((IParallelRangeRobustJob)jobs, amount);
	}

	public void ProcessNow(IParallelBulkRobustJob jobs, int amount)
	{
		ProcessNow((IParallelRangeRobustJob)jobs, amount);
	}

	public void ProcessSerialNow(IParallelRobustJob jobs, int amount)
	{
		ProcessSerialNow((IParallelRangeRobustJob)jobs, amount);
	}

	public void ProcessSerialNow(IParallelBulkRobustJob jobs, int amount)
	{
		ProcessSerialNow((IParallelRangeRobustJob)jobs, amount);
	}

	public WaitHandle Process(IParallelRobustJob jobs, int amount)
	{
		return Process((IParallelRangeRobustJob)jobs, amount);
	}

	public WaitHandle Process(IParallelBulkRobustJob jobs, int amount)
	{
		return Process((IParallelRangeRobustJob)jobs, amount);
	}

	public void ProcessNow(IParallelRangeRobustJob job, int amount)
	{
		if ((float)amount / (float)job.BatchSize <= (float)job.MinimumBatchParallel)
		{
			ProcessSerialNow(job, amount);
			return;
		}
		ParallelTracker parallelTracker = InternalProcess(job, amount);
		parallelTracker.Event.WaitHandle.WaitOne();
		_trackerPool.Return(parallelTracker);
	}

	public void ProcessSerialNow(IParallelRangeRobustJob jobs, int amount)
	{
		if (amount > 0)
		{
			jobs.ExecuteRange(0, amount);
		}
	}

	public WaitHandle Process(IParallelRangeRobustJob job, int amount)
	{
		return InternalProcess(job, amount).Event.WaitHandle;
	}

	private ParallelTracker InternalProcess(IParallelRangeRobustJob job, int amount)
	{
		int num = (int)MathF.Ceiling((float)amount / (float)job.BatchSize);
		int batchSize = job.BatchSize;
		ParallelTracker parallelTracker = _trackerPool.Get();
		parallelTracker.Event.Reset();
		if (amount <= 0)
		{
			parallelTracker.Event.Set();
			return parallelTracker;
		}
		parallelTracker.PendingTasks = num;
		for (int i = 0; i < num; i++)
		{
			int num2 = i * batchSize;
			int end = Math.Min(num2 + batchSize, amount);
			ThreadPool.UnsafeQueueUserWorkItem(GetParallelJob(job, num2, end, parallelTracker), preferLocal: true);
		}
		return parallelTracker;
	}
}
