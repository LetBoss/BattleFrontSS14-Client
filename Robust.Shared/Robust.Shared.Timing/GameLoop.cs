using System;
using System.Threading;
using Prometheus;
using Robust.Shared.Exceptions;
using Robust.Shared.Log;
using Robust.Shared.Profiling;

namespace Robust.Shared.Timing;

internal sealed class GameLoop : IGameLoop
{
	private static readonly TimeSpan DelayTime = TimeSpan.FromMilliseconds(1L);

	public const string ProfTextStartFrame = "Start Frame";

	private static readonly Histogram _frameTimeHistogram = Metrics.CreateHistogram("robust_game_loop_frametime", "Histogram of frametimes in ms", new HistogramConfiguration
	{
		Buckets = Histogram.ExponentialBuckets(0.001, 1.5, 10)
	});

	private readonly IGameTiming _timing;

	private TimeSpan _lastKeepUp;

	private readonly IRuntimeLog _runtimeLog;

	private readonly ProfManager _prof;

	private readonly ISawmill _sawmill;

	private readonly PrecisionSleep _precisionSleep;

	private int _tickExceptions;

	private const int MaxSoftLockExceptions = 10;

	public bool SingleStep { get; set; }

	public bool Running { get; set; }

	public int MaxQueuedTicks { get; set; } = 5;

	public TimeSpan LimitMinFrameTime { get; set; }

	public bool DetectSoftLock { get; set; }

	public bool EnableMetrics { get; set; }

	public SleepMode SleepMode { get; set; }

	public event EventHandler<FrameEventArgs>? Input;

	public event EventHandler<FrameEventArgs>? Tick;

	public event EventHandler<FrameEventArgs>? Update;

	public event EventHandler<FrameEventArgs>? Render;

	public GameLoop(IGameTiming timing, IRuntimeLog runtimeLog, ProfManager prof, ISawmill sawmill, GameLoopOptions options)
	{
		_timing = timing;
		_runtimeLog = runtimeLog;
		_prof = prof;
		_sawmill = sawmill;
		_precisionSleep = (options.Precise ? PrecisionSleep.Create() : new PrecisionSleepUniversal());
	}

	public void Run()
	{
		if (_timing.TickRate <= 0)
		{
			throw new InvalidOperationException("TickRate must be greater than 0.");
		}
		Running = true;
		while (Running)
		{
			long start = _prof.WriteValue("Start Frame", ProfData.Int64(_timing.CurFrame));
			long startIndex = _prof.WriteGroupStart();
			ProfSampler sampler = ProfSampler.StartNew();
			int num = GC.CollectionCount(0);
			int num2 = GC.CollectionCount(1);
			int num3 = GC.CollectionCount(2);
			_timing.StartFrame();
			TimeSpan timeSpan = TimeSpan.FromTicks(_timing.TickPeriod.Ticks * MaxQueuedTicks);
			TimeSpan timeSpan2 = _timing.RealTime - _timing.LastTick;
			if (timeSpan2 > timeSpan)
			{
				timeSpan2 = timeSpan;
				_timing.LastTick = _timing.RealTime - timeSpan;
				if ((_timing.RealTime - _lastKeepUp).TotalSeconds >= 15.0)
				{
					GameLoopEventSource.Log.CannotKeepUp();
					_sawmill.Warning("MainLoop: Cannot keep up!");
					_lastKeepUp = _timing.RealTime;
				}
			}
			FrameEventArgs e = new FrameEventArgs((float)_timing.RealFrameTime.TotalSeconds);
			GameLoopEventSource.Log.InputStart();
			try
			{
				using (_prof.Group("Input"))
				{
					this.Input?.Invoke(this, e);
				}
			}
			catch (Exception exception)
			{
				_runtimeLog.LogException(exception, "GameLoop Input");
			}
			GameLoopEventSource.Log.InputStop();
			_timing.InSimulation = true;
			TimeSpan timeSpan3 = _timing.CalcAdjustedTickPeriod();
			using (_prof.Group("Ticks"))
			{
				int num4 = 0;
				while (timeSpan2 >= timeSpan3)
				{
					timeSpan2 -= timeSpan3;
					_timing.LastTick += timeSpan3;
					if (_timing.Paused)
					{
						continue;
					}
					_timing.TickRemainder = timeSpan2 / _timing.TimeScale;
					num4++;
					FrameEventArgs e2 = new FrameEventArgs((float)_timing.FrameTime.TotalSeconds);
					bool flag = false;
					try
					{
						GameLoopEventSource.Log.TickStart(_timing.CurTick.Value);
						using (_prof.Group("Tick"))
						{
							_prof.WriteValue("Tick", ProfData.Int64(_timing.CurTick.Value));
							if (EnableMetrics)
							{
								ITimer val = TimerExtensions.NewTimer((IObserver)(object)_frameTimeHistogram);
								try
								{
									this.Tick?.Invoke(this, e2);
								}
								finally
								{
									((IDisposable)val)?.Dispose();
								}
							}
							else
							{
								this.Tick?.Invoke(this, e2);
							}
							GameLoopEventSource.Log.TickStop(_timing.CurTick.Value);
						}
					}
					catch (Exception exception2)
					{
						flag = true;
						_runtimeLog.LogException(exception2, "GameLoop Tick");
						_tickExceptions++;
						if (_tickExceptions > 10 && DetectSoftLock)
						{
							_sawmill.Fatal("MainLoop: 10 consecutive exceptions inside GameLoop Tick, shutting down!");
							Running = false;
						}
					}
					if (!flag)
					{
						_tickExceptions = 0;
					}
					_timing.CurTick = new GameTick(_timing.CurTick.Value + 1);
					timeSpan3 = _timing.CalcAdjustedTickPeriod();
					if (SingleStep)
					{
						_timing.Paused = true;
					}
				}
				_prof.WriteValue("Tick count", ProfData.Int32(num4));
			}
			if (!_timing.Paused)
			{
				_timing.TickRemainder = timeSpan2 / _timing.TimeScale;
			}
			_timing.InSimulation = false;
			GameLoopEventSource.Log.UpdateStart();
			try
			{
				using (_prof.Group("Update"))
				{
					this.Update?.Invoke(this, e);
				}
			}
			catch (Exception exception3)
			{
				_runtimeLog.LogException(exception3, "GameLoop Update");
			}
			GameLoopEventSource.Log.UpdateStop();
			try
			{
				using (_prof.Group("Render"))
				{
					this.Render?.Invoke(this, e);
				}
			}
			catch (Exception exception4)
			{
				_runtimeLog.LogException(exception4, "GameLoop Render");
			}
			using (_prof.Group("GC Overview"))
			{
				_prof.WriteValue("Gen 0 Count", ProfData.Int32(GC.CollectionCount(0) - num));
				_prof.WriteValue("Gen 1 Count", ProfData.Int32(GC.CollectionCount(1) - num2));
				_prof.WriteValue("Gen 2 Count", ProfData.Int32(GC.CollectionCount(2) - num3));
			}
			_prof.WriteGroupEnd(startIndex, "Frame", in sampler);
			_prof.MarkIndex(start, ProfIndexType.Frame);
			GameLoopEventSource.Log.SleepStart();
			switch (SleepMode)
			{
			case SleepMode.Yield:
				Thread.Sleep(0);
				break;
			case SleepMode.Delay:
			{
				TimeSpan timeSpan6 = _timing.LastTick + _timing.TickPeriod - _timing.RealTime;
				if (timeSpan6 > DelayTime)
				{
					timeSpan6 = DelayTime;
				}
				if (timeSpan6.Ticks > 0)
				{
					_precisionSleep.Sleep(timeSpan6);
				}
				break;
			}
			case SleepMode.Limit:
			{
				TimeSpan timeSpan4 = _timing.RealFrameTime - LimitMinFrameTime;
				if (timeSpan4.Ticks < 0)
				{
					timeSpan4 = TimeSpan.Zero;
				}
				TimeSpan timeSpan5 = _timing.RealTime - _timing.FrameStartTime;
				TimeSpan time = LimitMinFrameTime - timeSpan5 - timeSpan4;
				if (time.Ticks > 0)
				{
					RStopwatch.StartNew();
					_precisionSleep.Sleep(time);
				}
				break;
			}
			}
			GameLoopEventSource.Log.SleepStop();
		}
	}
}
