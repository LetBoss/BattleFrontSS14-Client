using System;

namespace Robust.Shared.Timing;

internal abstract class PrecisionSleep : IDisposable
{
	public abstract void Sleep(TimeSpan time);

	public static PrecisionSleep Create()
	{
		if (OperatingSystem.IsWindows() && Environment.OSVersion.Version.Build >= 17134)
		{
			return new PrecisionSleepWindowsHighResolution();
		}
		if (OperatingSystem.IsLinux())
		{
			return new PrecisionSleepLinuxNanosleep();
		}
		return new PrecisionSleepUniversal();
	}

	public virtual void Dispose()
	{
	}
}
