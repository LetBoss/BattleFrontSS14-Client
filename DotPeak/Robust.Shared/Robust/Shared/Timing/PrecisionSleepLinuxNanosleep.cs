// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Timing.PrecisionSleepLinuxNanosleep
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Robust.Shared.Timing;

internal sealed class PrecisionSleepLinuxNanosleep : PrecisionSleep
{
  public override unsafe void Sleep(TimeSpan time)
  {
    PrecisionSleepLinuxNanosleep.timespec timespec1;
    long result;
    timespec1.tv_sec = Math.DivRem(time.Ticks, 10000000L, out result);
    timespec1.tv_nsec = result * 100L;
    PrecisionSleepLinuxNanosleep.timespec timespec2;
    for (; PrecisionSleepLinuxNanosleep.nanosleep(&timespec1, &timespec2) != 0; timespec1 = timespec2)
    {
      int lastSystemError = Marshal.GetLastSystemError();
      if (lastSystemError != 4)
        throw new Exception($"nanosleep failed: {lastSystemError}");
    }
  }

  [DllImport("libc.so.6", SetLastError = true)]
  private static extern unsafe int nanosleep(
    PrecisionSleepLinuxNanosleep.timespec* req,
    PrecisionSleepLinuxNanosleep.timespec* rem);

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
}
