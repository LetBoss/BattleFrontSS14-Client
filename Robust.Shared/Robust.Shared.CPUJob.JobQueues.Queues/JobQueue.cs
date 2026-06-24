using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.Timing;

namespace Robust.Shared.CPUJob.JobQueues.Queues;

[Virtual]
public class JobQueue
{
	private readonly IStopwatch _stopwatch;

	private readonly Queue<IJob> _pendingQueue = new Queue<IJob>();

	private readonly List<IJob> _waitingJobs = new List<IJob>();

	public virtual double MaxTime { get; } = 0.002;

	public JobQueue(double maxTime)
		: this(new Stopwatch())
	{
		MaxTime = maxTime;
	}

	public JobQueue()
		: this(new Stopwatch())
	{
	}

	public JobQueue(IStopwatch stopwatch)
	{
		_stopwatch = stopwatch;
	}

	public void EnqueueJob(IJob job)
	{
		_pendingQueue.Enqueue(job);
	}

	public void Process()
	{
		foreach (IJob waitingJob in _waitingJobs)
		{
			if (waitingJob.Status != JobStatus.Waiting)
			{
				_pendingQueue.Enqueue(waitingJob);
			}
		}
		_waitingJobs.RemoveAll((IJob p) => p.Status != JobStatus.Waiting);
		_stopwatch.Restart();
		IJob result;
		while (_stopwatch.Elapsed.TotalSeconds < MaxTime && _pendingQueue.TryDequeue(out result))
		{
			result.Run();
			switch (result.Status)
			{
			case JobStatus.Finished:
				break;
			case JobStatus.Waiting:
				_waitingJobs.Add(result);
				break;
			default:
				_pendingQueue.Enqueue(result);
				break;
			}
		}
	}
}
