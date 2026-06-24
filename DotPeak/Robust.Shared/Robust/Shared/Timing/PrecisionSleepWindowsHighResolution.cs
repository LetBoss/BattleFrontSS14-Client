// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Timing.PrecisionSleepWindowsHighResolution
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;

#nullable disable
namespace Robust.Shared.Timing;

internal sealed class PrecisionSleepWindowsHighResolution : PrecisionSleep
{
  private HANDLE _timerHandle;

  public unsafe PrecisionSleepWindowsHighResolution()
  {
    this._timerHandle = TerraFX.Interop.Windows.Windows.CreateWaitableTimerExW((SECURITY_ATTRIBUTES*) null, (char*) null, 2U, 2031619U);
    if (!(this._timerHandle == HANDLE.NULL))
      return;
    Marshal.ThrowExceptionForHR((int) TerraFX.Interop.Windows.Windows.HRESULT_FROM_WIN32(Marshal.GetLastSystemError()));
  }

  public override unsafe void Sleep(TimeSpan time)
  {
    LARGE_INTEGER largeInteger;
    largeInteger.QuadPart = -time.Ticks;
    // ISSUE: cast to a function pointer type
    if (!(bool) TerraFX.Interop.Windows.Windows.SetWaitableTimer(this._timerHandle, &largeInteger, 0, (__FnPtr<void (void*, uint, uint)>) IntPtr.Zero, (void*) null, BOOL.FALSE))
      Marshal.ThrowExceptionForHR((int) TerraFX.Interop.Windows.Windows.HRESULT_FROM_WIN32(Marshal.GetLastSystemError()));
    if (TerraFX.Interop.Windows.Windows.WaitForSingleObject(this._timerHandle, uint.MaxValue) == uint.MaxValue)
      Marshal.ThrowExceptionForHR((int) TerraFX.Interop.Windows.Windows.HRESULT_FROM_WIN32(Marshal.GetLastSystemError()));
    GC.KeepAlive((object) this);
  }

  private void DisposeCore()
  {
    TerraFX.Interop.Windows.Windows.CloseHandle(this._timerHandle);
    this._timerHandle = new HANDLE();
  }

  public override void Dispose()
  {
    this.DisposeCore();
    GC.SuppressFinalize((object) this);
  }

  ~PrecisionSleepWindowsHighResolution() => this.DisposeCore();
}
