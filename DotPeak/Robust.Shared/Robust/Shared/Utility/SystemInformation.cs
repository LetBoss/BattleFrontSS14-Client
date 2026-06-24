// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.SystemInformation
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.Wasm;
using System.Runtime.Intrinsics.X86;
using System.Text;

#nullable enable
namespace Robust.Shared.Utility;

internal static class SystemInformation
{
  public static string GetProcessorModel()
  {
    if (X86Base.IsSupported)
    {
      string processorModelX86 = SystemInformation.GetProcessorModelX86();
      if (processorModelX86 != null)
        return processorModelX86;
    }
    if (OperatingSystem.IsMacOS())
    {
      string processorModelMacOs = SystemInformation.GetProcessorModelMacOS();
      if (processorModelMacOs != null)
        return processorModelMacOs;
    }
    else if (OperatingSystem.IsWindows())
    {
      string processorModelWindows = SystemInformation.GetProcessorModelWindows();
      if (processorModelWindows != null)
        return processorModelWindows;
    }
    else if (OperatingSystem.IsLinux())
    {
      string processorModelLinux = SystemInformation.GetProcessorModelLinux();
      if (processorModelLinux != null)
        return processorModelLinux;
    }
    return "Unknown processor model";
  }

  private static string? GetProcessorModelX86()
  {
    if (X86Base.CpuId(134217728 /*0x08000000*/, 0).Eax < -2147483644 /*0x80000004*/)
      return (string) null;
    Span<int> span = stackalloc int[12];
    for (int index = 0; index < 3; ++index)
    {
      (int Eax, int Ebx, int Ecx, int Edx) = X86Base.CpuId(index - 2147483646, 0);
      span[index * 4] = Eax;
      span[index * 4 + 1] = Ebx;
      span[index * 4 + 2] = Ecx;
      span[index * 4 + 3] = Edx;
    }
    return Encoding.UTF8.GetString((ReadOnlySpan<byte>) MemoryMarshal.Cast<int, byte>(span).TrimEnd<byte>((byte) 0)).TrimEnd();
  }

  private static unsafe string? GetProcessorModelMacOS()
  {
    // ISSUE: reference to a compiler-generated field
    fixed (byte* name = &new ReadOnlySpan<byte>((void*) &\u003CPrivateImplementationDetails\u003E.\u0039E358F197F24B02A012F7F1A5353031A36076A186315615CBDC1CCD53E6460EA, 24).GetPinnableReference())
    {
      IntPtr zero = IntPtr.Zero;
      if (SystemInformation.sysctlbyname(name, (void*) null, &zero, (void*) null, IntPtr.Zero) != 0)
        throw new Win32Exception(Marshal.GetLastPInvokeError());
      Span<byte> bytes = stackalloc byte[(int) zero];
      int num;
      fixed (byte* oldp = &bytes.GetPinnableReference())
        num = SystemInformation.sysctlbyname(name, (void*) oldp, &zero, (void*) null, IntPtr.Zero);
      if (num != 0)
        throw new Win32Exception(Marshal.GetLastPInvokeError());
      return Encoding.UTF8.GetString((ReadOnlySpan<byte>) bytes).TrimEnd();
    }
  }

  [DllImport("libc", SetLastError = true)]
  private static extern unsafe int sysctlbyname(
    byte* name,
    void* oldp,
    IntPtr* oldlenp,
    void* newp,
    IntPtr newlen);

  private static string? GetProcessorModelWindows()
  {
    using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = new ManagementObjectSearcher("select Name from Win32_Processor").Get().GetEnumerator())
    {
      if (enumerator.MoveNext())
        return (string) enumerator.Current["Name"];
    }
    return (string) null;
  }

  private static string? GetProcessorModelLinux()
  {
    using (StreamReader streamReader = new StreamReader("/proc/cpuinfo"))
    {
      string[] strArray;
      do
      {
        string str = streamReader.ReadLine();
        if (str != null)
          strArray = str.Split(':', 2);
        else
          goto label_4;
      }
      while (strArray.Length != 2 || !(strArray[0].Trim() == "model name"));
      return strArray[1].Trim();
label_4:
      return (string) null;
    }
  }

  public static List<string> GetIntrinsics()
  {
    List<string> intrinsics = new List<string>();
    if (System.Runtime.Intrinsics.X86.Aes.IsSupported)
      intrinsics.Add("X86Aes");
    if (Avx.IsSupported)
      intrinsics.Add("Avx");
    if (Avx2.IsSupported)
      intrinsics.Add("Avx2");
    if (Avx512BW.IsSupported)
      intrinsics.Add("Avx512BW");
    if (Avx512BW.VL.IsSupported)
      intrinsics.Add("Avx512BW.VL");
    if (Avx512CD.IsSupported)
      intrinsics.Add("Avx512CD");
    if (Avx512CD.VL.IsSupported)
      intrinsics.Add("Avx512CD.VL");
    if (Avx512DQ.IsSupported)
      intrinsics.Add("Avx512DQ");
    if (Avx512DQ.VL.IsSupported)
      intrinsics.Add("Avx512DQ.VL");
    if (Avx512F.IsSupported)
      intrinsics.Add("Avx512F");
    if (Avx512F.VL.IsSupported)
      intrinsics.Add("Avx512F.VL");
    if (Avx512Vbmi.IsSupported)
      intrinsics.Add("Avx512Vbmi");
    if (Avx512Vbmi.VL.IsSupported)
      intrinsics.Add("Avx512Vbmi.VL");
    if (Avx10v1.IsSupported)
      intrinsics.Add("Avx10v1");
    if (Avx10v1.V512.IsSupported)
      intrinsics.Add("Avx10v1.V512");
    if (Avx10v2.IsSupported)
      intrinsics.Add("Avx10v2");
    if (Avx10v2.V512.IsSupported)
      intrinsics.Add("Avx10v2.V512");
    if (Bmi1.IsSupported)
      intrinsics.Add("Bmi1");
    if (Bmi2.IsSupported)
      intrinsics.Add("Bmi2");
    if (Fma.IsSupported)
      intrinsics.Add("Fma");
    if (Lzcnt.IsSupported)
      intrinsics.Add("Lzcnt");
    if (Pclmulqdq.IsSupported)
      intrinsics.Add("Pclmulqdq");
    if (Popcnt.IsSupported)
      intrinsics.Add("Popcnt");
    if (Sse.IsSupported)
      intrinsics.Add("Sse");
    if (Sse2.IsSupported)
      intrinsics.Add("Sse2");
    if (Sse3.IsSupported)
      intrinsics.Add("Sse3");
    if (Ssse3.IsSupported)
      intrinsics.Add("Ssse3");
    if (Sse41.IsSupported)
      intrinsics.Add("Sse41");
    if (Sse42.IsSupported)
      intrinsics.Add("Sse42");
    if (X86Base.IsSupported)
      intrinsics.Add("X86Base");
    if (AdvSimd.IsSupported)
      intrinsics.Add("AdvSimd");
    if (System.Runtime.Intrinsics.Arm.Aes.IsSupported)
      intrinsics.Add("ArmAes");
    if (ArmBase.IsSupported)
      intrinsics.Add("ArmBase");
    if (Crc32.IsSupported)
      intrinsics.Add("Crc32");
    if (Dp.IsSupported)
      intrinsics.Add("Dp");
    if (Rdm.IsSupported)
      intrinsics.Add("Rdm");
    if (Sha1.IsSupported)
      intrinsics.Add("Sha1");
    if (Sha256.IsSupported)
      intrinsics.Add("Sha256");
    if (PackedSimd.IsSupported)
      intrinsics.Add("PackedSimd");
    return intrinsics;
  }
}
