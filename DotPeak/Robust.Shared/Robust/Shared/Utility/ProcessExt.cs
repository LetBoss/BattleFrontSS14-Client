// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.ProcessExt
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TerraFX.Interop.Windows;

#nullable enable
namespace Robust.Shared.Utility;

internal static class ProcessExt
{
  [MethodImpl(MethodImplOptions.NoInlining)]
  public static unsafe long GetPrivateMemorySize64NotSlowHolyFuckingShitMicrosoft(
    this Process process)
  {
    if (!OperatingSystem.IsWindows())
      return process.PrivateMemorySize64;
    PROCESS_MEMORY_COUNTERS_EX memoryCountersEx;
    // ISSUE: function pointer call
    return __calli(TerraFX.Interop.Windows.Windows.GetProcessMemoryInfo)((HANDLE) process.Handle, (PROCESS_MEMORY_COUNTERS*) &memoryCountersEx, (uint) sizeof (PROCESS_MEMORY_COUNTERS_EX)) == (BOOL) 0 ? 0L : (long) (ulong) memoryCountersEx.PrivateUsage;
  }
}
