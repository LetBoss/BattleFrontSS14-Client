using System;
using Robust.Shared.Analyzers;
using Robust.Shared.IoC;

namespace Robust.Shared.Timing;

[NotContentImplementable]
public interface IGameTiming
{
	bool InSimulation { get; set; }

	bool Paused { get; set; }

	TimeSpan CurTime { get; }

	TimeSpan RealTime { get; }

	TimeSpan FrameStartTime { get; }

	TimeSpan ServerTime { get; }

	TimeSpan FrameTime { get; }

	TimeSpan RealFrameTime { get; }

	TimeSpan RealFrameTimeAvg { get; }

	TimeSpan RealFrameTimeStdDev { get; }

	uint CurFrame { get; set; }

	double FramesPerSecondAvg { get; }

	GameTick CurTick { get; set; }

	TimeSpan LastTick { get; set; }

	ushort TickRate { get; set; }

	float TimeScale { get; set; }

	(TimeSpan, GameTick) TimeBase { get; set; }

	TimeSpan TickPeriod { get; }

	TimeSpan TickRemainder { get; set; }

	TimeSpan TickRemainderRealtime { get; }

	ushort TickFraction
	{
		get
		{
			if (InSimulation)
			{
				return ushort.MaxValue;
			}
			return (ushort)(65535.0 * TickRemainder.TotalSeconds / TickPeriod.TotalSeconds);
		}
	}

	float TickTimingAdjustment { get; set; }

	bool IsFirstTimePredicted { get; }

	bool InPrediction { get; }

	bool ApplyingState { get; }

	string TickStamp => $"{CurTick}, predFirst: {IsFirstTimePredicted}, tickRem: {TickRemainder.TotalSeconds}, sim: {InSimulation}";

	static string TickStampStatic => IoCManager.Resolve<IGameTiming>().TickStamp;

	TimeSpan CalcAdjustedTickPeriod();

	void StartFrame();

	void ResetSimTime();

	void ResetSimTime((TimeSpan, GameTick) timeBase);

	void SetTickRateAt(ushort tickRate, GameTick atTick);

	TimeSpan RealLocalToServer(TimeSpan local);

	TimeSpan RealServerToLocal(TimeSpan server);
}
