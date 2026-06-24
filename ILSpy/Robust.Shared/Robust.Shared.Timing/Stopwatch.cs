using System;
using System.Diagnostics;

namespace Robust.Shared.Timing;

public sealed class Stopwatch : IStopwatch
{
	private readonly System.Diagnostics.Stopwatch _stopwatch;

	public TimeSpan Elapsed => _stopwatch.Elapsed;

	public Stopwatch()
	{
		_stopwatch = new System.Diagnostics.Stopwatch();
	}

	public void Start()
	{
		_stopwatch.Start();
	}

	public void Restart()
	{
		_stopwatch.Restart();
	}
}
