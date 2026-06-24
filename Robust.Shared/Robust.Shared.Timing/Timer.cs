using System;
using System.Threading;
using System.Threading.Tasks;
using Robust.Shared.Exceptions;
using Robust.Shared.IoC;

namespace Robust.Shared.Timing;

public sealed class Timer
{
	private float _timeCounter;

	public int Time { get; }

	public bool IsRepeating { get; }

	public bool IsActive { get; private set; } = true;

	public Action OnFired { get; }

	public Timer(int milliseconds, bool isRepeating, Action onFired)
	{
		_timeCounter = (Time = milliseconds);
		IsRepeating = isRepeating;
		OnFired = onFired;
	}

	public void Update(float frameTime, IRuntimeLog runtimeLog)
	{
		if (!IsActive)
		{
			return;
		}
		_timeCounter -= frameTime * 1000f;
		if (_timeCounter <= 0f)
		{
			try
			{
				OnFired();
			}
			catch (Exception exception)
			{
				runtimeLog.LogException(exception, "Timer Callback");
			}
			if (IsRepeating)
			{
				_timeCounter += Time;
			}
			else
			{
				IsActive = false;
			}
		}
	}

	public static Task Delay(int milliseconds, CancellationToken cancellationToken = default(CancellationToken))
	{
		TaskCompletionSource<object?> tcs = new TaskCompletionSource<object>();
		Spawn(milliseconds, delegate
		{
			tcs.SetResult(null);
		}, cancellationToken);
		return tcs.Task;
	}

	public static Task Delay(TimeSpan duration, CancellationToken cancellationToken = default(CancellationToken))
	{
		return Delay((int)duration.TotalMilliseconds, cancellationToken);
	}

	public static void Spawn(int milliseconds, Action onFired, CancellationToken cancellationToken = default(CancellationToken))
	{
		Timer timer = new Timer(milliseconds, isRepeating: false, onFired);
		IoCManager.Resolve<ITimerManager>().AddTimer(timer, cancellationToken);
	}

	public static void Spawn(TimeSpan duration, Action onFired, CancellationToken cancellationToken = default(CancellationToken))
	{
		Spawn((int)duration.TotalMilliseconds, onFired, cancellationToken);
	}

	public static void SpawnRepeating(int milliseconds, Action onFired, CancellationToken cancellationToken)
	{
		Timer timer = new Timer(milliseconds, isRepeating: true, onFired);
		IoCManager.Resolve<ITimerManager>().AddTimer(timer, cancellationToken);
	}

	public static void SpawnRepeating(TimeSpan duration, Action onFired, CancellationToken cancellationToken)
	{
		SpawnRepeating((int)duration.TotalMilliseconds, onFired, cancellationToken);
	}
}
