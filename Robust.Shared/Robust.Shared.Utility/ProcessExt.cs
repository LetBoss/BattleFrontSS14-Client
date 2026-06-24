using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TerraFX.Interop.Windows;

namespace Robust.Shared.Utility;

internal static class ProcessExt
{
	[MethodImpl(MethodImplOptions.NoInlining)]
	public unsafe static long GetPrivateMemorySize64NotSlowHolyFuckingShitMicrosoft(this Process process)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (!OperatingSystem.IsWindows())
		{
			return process.PrivateMemorySize64;
		}
		Unsafe.SkipInit(out PROCESS_MEMORY_COUNTERS_EX val);
		if (Windows.GetProcessMemoryInfo((HANDLE)(IntPtr)process.Handle, (PROCESS_MEMORY_COUNTERS*)(&val), (uint)Unsafe.SizeOf<PROCESS_MEMORY_COUNTERS_EX>()) == BOOL.op_Implicit(0))
		{
			return 0L;
		}
		return (long)(nuint)val.PrivateUsage;
	}
}
