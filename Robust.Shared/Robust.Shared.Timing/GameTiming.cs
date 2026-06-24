using System;
using System.Linq;
using Robust.Shared.Analyzers;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Robust.Shared.Timing;

[Virtual]
public class GameTiming : IGameTiming
{
	private const int NumFrames = 60;

	private readonly IStopwatch _realTimer = new Stopwatch();

	private readonly long[] _realFrameTimes = new long[60];

	private int _frameIdx;

	private TimeSpan _lastRealTime;

	private ushort _tickRate;

	private TimeSpan _tickRemainder;

	public bool InSimulation { get; set; }

	public bool Paused { get; set; }

	public (TimeSpan, GameTick) TimeBase { get; set; } = (TimeSpan.Zero, GameTick.First);

	public TimeSpan CurTime
	{
		get
		{
			var (timeSpan, gameTick) = TimeBase;
			timeSpan += TickPeriod.Mul(CurTick.Value - gameTick.Value);
			if (!InSimulation)
			{
				return timeSpan + TickRemainder;
			}
			return timeSpan;
		}
	}

	public TimeSpan RealTime => _realTimer.Elapsed;

	public TimeSpan FrameStartTime => _lastRealTime;

	public virtual TimeSpan ServerTime => TimeSpan.Zero;

	public TimeSpan FrameTime => CalcFrameTime();

	public TimeSpan RealFrameTime { get; private set; }

	public TimeSpan RealFrameTimeAvg => TimeSpan.FromTicks((long)_realFrameTimes.Average());

	public TimeSpan RealFrameTimeStdDev => CalcRftStdDev();

	public double FramesPerSecondAvg => CalcFpsAvg();

	public GameTick CurTick { get; set; } = new GameTick(1u);

	public TimeSpan LastTick { get; set; }

	public ushort TickRate
	{
		get
		{
			return _tickRate;
		}
		set
		{
			SetTickRateAt(value, CurTick);
		}
	}

	public float TimeScale { get; set; } = 1f;

	public TimeSpan TickPeriod => TimeSpan.FromTicks((long)(1.0 / (double)(int)TickRate * 10000000.0));

	public TimeSpan TickRemainder
	{
		get
		{
			return _tickRemainder;
		}
		set
		{
			_tickRemainder = value;
		}
	}

	public TimeSpan TickRemainderRealtime => TickRemainder * TimeScale;

	public uint CurFrame { get; set; } = 1u;

	public float TickTimingAdjustment { get; set; }

	public bool IsFirstTimePredicted { get; protected set; } = true;

	public virtual bool InPrediction => false;

	public bool ApplyingState { get; protected set; }

	public GameTiming()
	{
		_realTimer.Start();
		Paused = true;
		TickRate = 60;
	}

	public TimeSpan CalcAdjustedTickPeriod()
	{
		float num = MathHelper.Clamp(TickTimingAdjustment, -0.99f, 0.99f);
		return TickPeriod * (1f - num) * TimeScale;
	}

	public void StartFrame()
	{
		TimeSpan realTime = RealTime;
		RealFrameTime = realTime - _lastRealTime;
		_lastRealTime = realTime;
		_frameIdx = (1 + _frameIdx) % _realFrameTimes.Length;
		_realFrameTimes[_frameIdx] = RealFrameTime.Ticks;
	}

	private TimeSpan CalcFrameTime()
	{
		if (InSimulation)
		{
			return TimeSpan.FromTicks(TickPeriod.Ticks);
		}
		if (!Paused)
		{
			return RealFrameTime;
		}
		return TimeSpan.Zero;
	}

	private void CacheCurTime(GameTick atTick)
	{
		(TimeSpan, GameTick) timeBase = TimeBase;
		TimeSpan item = timeBase.Item1;
		GameTick item2 = timeBase.Item2;
		TimeSpan item3 = item + TickPeriod.Mul(atTick.Value - item2.Value);
		TimeBase = (item3, atTick);
	}

	public void ResetSimTime()
	{
		ResetSimTime((TimeSpan.Zero, GameTick.First));
	}

	public void ResetSimTime((TimeSpan, GameTick) timeBase)
	{
		TimeBase = timeBase;
		CurTick = GameTick.First;
		TickRemainder = TimeSpan.Zero;
		Paused = true;
	}

	public void SetTickRateAt(ushort tickRate, GameTick atTick)
	{
		if (_tickRate != 0)
		{
			CacheCurTime(atTick);
		}
		_tickRate = tickRate;
	}

	public virtual TimeSpan RealLocalToServer(TimeSpan local)
	{
		return TimeSpan.Zero;
	}

	public virtual TimeSpan RealServerToLocal(TimeSpan server)
	{
		return TimeSpan.Zero;
	}

	protected virtual TimeSpan? GetServerOffset()
	{
		return null;
	}

	private double CalcFpsAvg()
	{
		return 1.0 / (_realFrameTimes.Average() / 10000000.0);
	}

	private TimeSpan CalcRftStdDev()
	{
		long num = _realFrameTimes.Sum();
		int num2 = _realFrameTimes.Length;
		double num3 = (double)num / (double)num2;
		double num4 = 0.0;
		for (int i = 0; i < num2; i++)
		{
			if (_realFrameTimes[i] != 0L)
			{
				double num5 = (double)_realFrameTimes[i] - num3;
				num4 += num5 * num5;
			}
		}
		return TimeSpan.FromTicks((long)Math.Sqrt(num4 / (double)(num2 - 1)));
	}

	internal static bool IsTimescaleValid(float scale)
	{
		if (scale > 0f && float.IsNormal(scale))
		{
			return float.IsFinite(scale);
		}
		return false;
	}
}
