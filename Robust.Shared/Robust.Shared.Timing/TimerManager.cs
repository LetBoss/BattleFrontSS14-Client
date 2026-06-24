using System.Collections.Generic;
using System.Threading;
using Robust.Shared.Exceptions;
using Robust.Shared.IoC;

namespace Robust.Shared.Timing;

internal sealed class TimerManager : ITimerManager
{
	[Dependency]
	private readonly IRuntimeLog _runtimeLog;

	private readonly List<(Timer, CancellationToken)> _timers = new List<(Timer, CancellationToken)>();

	public void AddTimer(Timer timer, CancellationToken cancellationToken = default(CancellationToken))
	{
		_timers.Add((timer, cancellationToken));
	}

	public void UpdateTimers(FrameEventArgs frameEventArgs)
	{
		for (int i = 0; i < _timers.Count; i++)
		{
			var (timer, cancellationToken) = _timers[i];
			if (!cancellationToken.IsCancellationRequested)
			{
				timer.Update(frameEventArgs.DeltaSeconds, _runtimeLog);
			}
		}
		_timers.RemoveAll(((Timer, CancellationToken) tuple2) => !tuple2.Item1.IsActive || tuple2.Item2.IsCancellationRequested);
	}
}
