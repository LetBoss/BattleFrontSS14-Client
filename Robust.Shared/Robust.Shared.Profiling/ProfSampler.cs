using System;
using Robust.Shared.Timing;

namespace Robust.Shared.Profiling;

public struct ProfSampler
{
	public RStopwatch Stopwatch;

	private long _alloc;

	public readonly long ElapsedAlloc
	{
		get
		{
			if (!Stopwatch.IsRunning)
			{
				return _alloc;
			}
			return GC.GetAllocatedBytesForCurrentThread() - _alloc;
		}
	}

	public readonly TimeSpan Elapsed => Stopwatch.Elapsed;

	public static ProfSampler StartNew()
	{
		ProfSampler result = default(ProfSampler);
		result.Start();
		return result;
	}

	public void Start()
	{
		if (!Stopwatch.IsRunning)
		{
			Stopwatch.Start();
			long allocatedBytesForCurrentThread = GC.GetAllocatedBytesForCurrentThread();
			_alloc = allocatedBytesForCurrentThread - _alloc;
		}
	}

	public void Restart()
	{
		Stopwatch.Restart();
		_alloc = GC.GetAllocatedBytesForCurrentThread();
	}

	public void Stop()
	{
		if (Stopwatch.IsRunning)
		{
			_alloc = ElapsedAlloc;
			Stopwatch.Stop();
		}
	}

	public void Reset()
	{
		this = default(ProfSampler);
	}
}
