using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Robust.Shared.Timing;

internal sealed class PrecisionSleepLinuxNanosleep : PrecisionSleep
{
	private struct timespec
	{
		public long tv_sec;

		public long tv_nsec;
	}

	private struct timeval
	{
		public long tv_sec;

		public long tv_usec;
	}

	public unsafe override void Sleep(TimeSpan time)
	{
		Unsafe.SkipInit(out timespec timespec2);
		timespec2.tv_sec = Math.DivRem(time.Ticks, 10000000L, out var result);
		timespec2.tv_nsec = result * 100;
		Unsafe.SkipInit(out timespec timespec3);
		int lastSystemError;
		while (true)
		{
			if (nanosleep(&timespec2, &timespec3) == 0)
			{
				return;
			}
			lastSystemError = Marshal.GetLastSystemError();
			if (lastSystemError != 4)
			{
				break;
			}
			timespec2 = timespec3;
		}
		throw new Exception($"nanosleep failed: {lastSystemError}");
	}

	[DllImport("libc.so.6", SetLastError = true)]
	private unsafe static extern int nanosleep(timespec* req, timespec* rem);
}
