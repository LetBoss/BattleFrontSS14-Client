using System;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Shared.Timing;

[Serializable]
public record struct StartEndTime(TimeSpan Start, TimeSpan End)
{
	public TimeSpan Length => End - Start;

	public float ProgressAt(TimeSpan time, bool clamp = true)
	{
		TimeSpan length = Length;
		if (length == default(TimeSpan))
		{
			return float.NaN;
		}
		float progress = (float)((time - Start) / length);
		if (clamp)
		{
			progress = MathHelper.Clamp01(progress);
		}
		return progress;
	}

	public static StartEndTime FromStartDuration(TimeSpan start, TimeSpan duration)
	{
		return new StartEndTime(start, start + duration);
	}

	public static StartEndTime FromStartDuration(TimeSpan start, float durationSeconds)
	{
		return new StartEndTime(start, start + TimeSpan.FromSeconds(durationSeconds));
	}

	public static StartEndTime FromCurTime(IGameTiming gameTiming, TimeSpan duration)
	{
		return FromStartDuration(gameTiming.CurTime, duration);
	}

	public static StartEndTime FromCurTime(IGameTiming gameTiming, float durationSeconds)
	{
		return FromStartDuration(gameTiming.CurTime, durationSeconds);
	}
}
