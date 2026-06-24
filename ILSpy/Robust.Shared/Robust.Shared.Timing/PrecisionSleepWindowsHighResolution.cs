using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;

namespace Robust.Shared.Timing;

internal sealed class PrecisionSleepWindowsHighResolution : PrecisionSleep
{
	private HANDLE _timerHandle;

	public unsafe PrecisionSleepWindowsHighResolution()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		_timerHandle = Windows.CreateWaitableTimerExW((SECURITY_ATTRIBUTES*)null, (char*)null, 2u, 2031619u);
		if (_timerHandle == HANDLE.NULL)
		{
			Marshal.ThrowExceptionForHR(HRESULT.op_Implicit(Windows.HRESULT_FROM_WIN32(Marshal.GetLastSystemError())));
		}
	}

	public unsafe override void Sleep(TimeSpan time)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		Unsafe.SkipInit(out LARGE_INTEGER val);
		val.QuadPart = -time.Ticks;
		if (!BOOL.op_Implicit(Windows.SetWaitableTimer(_timerHandle, &val, 0, (delegate* unmanaged<void*, uint, uint, void>)null, (void*)null, BOOL.FALSE)))
		{
			Marshal.ThrowExceptionForHR(HRESULT.op_Implicit(Windows.HRESULT_FROM_WIN32(Marshal.GetLastSystemError())));
		}
		if (Windows.WaitForSingleObject(_timerHandle, uint.MaxValue) == uint.MaxValue)
		{
			Marshal.ThrowExceptionForHR(HRESULT.op_Implicit(Windows.HRESULT_FROM_WIN32(Marshal.GetLastSystemError())));
		}
		GC.KeepAlive(this);
	}

	private void DisposeCore()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Windows.CloseHandle(_timerHandle);
		_timerHandle = default(HANDLE);
	}

	public override void Dispose()
	{
		DisposeCore();
		GC.SuppressFinalize(this);
	}

	~PrecisionSleepWindowsHighResolution()
	{
		DisposeCore();
	}
}
