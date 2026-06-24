// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Timing.PrecisionSleep
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Timing;

internal abstract class PrecisionSleep : IDisposable
{
  public abstract void Sleep(TimeSpan time);

  public static PrecisionSleep Create()
  {
    if (OperatingSystem.IsWindows() && Environment.OSVersion.Version.Build >= 17134)
      return (PrecisionSleep) new PrecisionSleepWindowsHighResolution();
    return OperatingSystem.IsLinux() ? (PrecisionSleep) new PrecisionSleepLinuxNanosleep() : (PrecisionSleep) new PrecisionSleepUniversal();
  }

  public virtual void Dispose()
  {
  }
}
