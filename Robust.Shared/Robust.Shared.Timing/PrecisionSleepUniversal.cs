using System;
using System.Threading;

namespace Robust.Shared.Timing;

internal sealed class PrecisionSleepUniversal : PrecisionSleep
{
	public override void Sleep(TimeSpan time)
	{
		Thread.Sleep(time);
	}
}
