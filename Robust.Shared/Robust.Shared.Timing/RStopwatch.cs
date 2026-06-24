using System;
using System.Diagnostics;

namespace Robust.Shared.Timing;

public struct RStopwatch
{
	private long _curTicks;

	private static readonly double TicksToTimeTicks = 10000000.0 / (double)System.Diagnostics.Stopwatch.Frequency;

	public bool IsRunning { get; private set; }

	public readonly long ElapsedTicks
	{
		get
		{
			if (!IsRunning)
			{
				return _curTicks;
			}
			return System.Diagnostics.Stopwatch.GetTimestamp() - _curTicks;
		}
	}

	public readonly TimeSpan Elapsed => new TimeSpan(ElapsedTimeTicks());

	public static RStopwatch StartNew()
	{
		RStopwatch result = default(RStopwatch);
		result.Start();
		return result;
	}

	public void Start()
	{
		if (!IsRunning)
		{
			long timestamp = System.Diagnostics.Stopwatch.GetTimestamp();
			_curTicks = timestamp - _curTicks;
			IsRunning = true;
		}
	}

	public void Restart()
	{
		IsRunning = true;
		_curTicks = System.Diagnostics.Stopwatch.GetTimestamp();
	}

	public void Stop()
	{
		if (IsRunning)
		{
			_curTicks = ElapsedTicks;
			IsRunning = false;
		}
	}

	public void Reset()
	{
		this = default(RStopwatch);
	}

	private readonly long ElapsedTimeTicks()
	{
		return (long)(TicksToTimeTicks * (double)ElapsedTicks);
	}
}
