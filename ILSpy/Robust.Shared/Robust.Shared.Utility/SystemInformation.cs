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

namespace Robust.Shared.Utility;

internal static class SystemInformation
{
	public static string GetProcessorModel()
	{
		if (X86Base.IsSupported)
		{
			string processorModelX = GetProcessorModelX86();
			if (processorModelX != null)
			{
				return processorModelX;
			}
		}
		if (OperatingSystem.IsMacOS())
		{
			string processorModelMacOS = GetProcessorModelMacOS();
			if (processorModelMacOS != null)
			{
				return processorModelMacOS;
			}
		}
		else if (OperatingSystem.IsWindows())
		{
			string processorModelWindows = GetProcessorModelWindows();
			if (processorModelWindows != null)
			{
				return processorModelWindows;
			}
		}
		else if (OperatingSystem.IsLinux())
		{
			string processorModelLinux = GetProcessorModelLinux();
			if (processorModelLinux != null)
			{
				return processorModelLinux;
			}
		}
		return "Unknown processor model";
	}

	private static string? GetProcessorModelX86()
	{
		if (X86Base.CpuId(134217728, 0).Eax < -2147483644)
		{
			return null;
		}
		Span<int> span = stackalloc int[12];
		for (int i = 0; i < 3; i++)
		{
			var (num, num2, num3, num4) = X86Base.CpuId(-2147483646 + i, 0);
			span[i * 4] = num;
			span[i * 4 + 1] = num2;
			span[i * 4 + 2] = num3;
			span[i * 4 + 3] = num4;
		}
		Span<byte> span2 = MemoryMarshal.Cast<int, byte>(span).TrimEnd((byte)0);
		return Encoding.UTF8.GetString(span2).TrimEnd();
	}

	private unsafe static string? GetProcessorModelMacOS()
	{
		fixed (byte* name = "machdep.cpu.brand_string"u8)
		{
			nint num = 0;
			if (sysctlbyname(name, null, &num, null, 0) != 0)
			{
				throw new Win32Exception(Marshal.GetLastPInvokeError());
			}
			Span<byte> span = stackalloc byte[(int)num];
			int num2;
			fixed (byte* oldp = span)
			{
				num2 = sysctlbyname(name, oldp, &num, null, 0);
			}
			if (num2 != 0)
			{
				throw new Win32Exception(Marshal.GetLastPInvokeError());
			}
			return Encoding.UTF8.GetString(span).TrimEnd();
		}
	}

	[DllImport("libc", SetLastError = true)]
	private unsafe static extern int sysctlbyname(byte* name, void* oldp, nint* oldlenp, void* newp, nint newlen);

	private static string? GetProcessorModelWindows()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		ManagementObjectEnumerator enumerator = new ManagementObjectSearcher("select Name from Win32_Processor").Get().GetEnumerator();
		try
		{
			if (enumerator.MoveNext())
			{
				return (string)((ManagementBaseObject)(ManagementObject)enumerator.Current)["Name"];
			}
		}
		finally
		{
			((IDisposable)enumerator)?.Dispose();
		}
		return null;
	}

	private static string? GetProcessorModelLinux()
	{
		using StreamReader streamReader = new StreamReader("/proc/cpuinfo");
		while (true)
		{
			string text = streamReader.ReadLine();
			if (text == null)
			{
				break;
			}
			string[] array = text.Split(':', 2);
			if (array.Length == 2 && array[0].Trim() == "model name")
			{
				return array[1].Trim();
			}
		}
		return null;
	}

	public static List<string> GetIntrinsics()
	{
		List<string> list = new List<string>();
		if (System.Runtime.Intrinsics.X86.Aes.IsSupported)
		{
			list.Add("X86Aes");
		}
		if (Avx.IsSupported)
		{
			list.Add("Avx");
		}
		if (Avx2.IsSupported)
		{
			list.Add("Avx2");
		}
		if (Avx512BW.IsSupported)
		{
			list.Add("Avx512BW");
		}
		if (Avx512BW.VL.IsSupported)
		{
			list.Add("Avx512BW.VL");
		}
		if (Avx512CD.IsSupported)
		{
			list.Add("Avx512CD");
		}
		if (Avx512CD.VL.IsSupported)
		{
			list.Add("Avx512CD.VL");
		}
		if (Avx512DQ.IsSupported)
		{
			list.Add("Avx512DQ");
		}
		if (Avx512DQ.VL.IsSupported)
		{
			list.Add("Avx512DQ.VL");
		}
		if (Avx512F.IsSupported)
		{
			list.Add("Avx512F");
		}
		if (Avx512F.VL.IsSupported)
		{
			list.Add("Avx512F.VL");
		}
		if (Avx512Vbmi.IsSupported)
		{
			list.Add("Avx512Vbmi");
		}
		if (Avx512Vbmi.VL.IsSupported)
		{
			list.Add("Avx512Vbmi.VL");
		}
		if (Avx10v1.IsSupported)
		{
			list.Add("Avx10v1");
		}
		if (Avx10v1.V512.IsSupported)
		{
			list.Add("Avx10v1.V512");
		}
		if (Avx10v2.IsSupported)
		{
			list.Add("Avx10v2");
		}
		if (Avx10v2.V512.IsSupported)
		{
			list.Add("Avx10v2.V512");
		}
		if (Bmi1.IsSupported)
		{
			list.Add("Bmi1");
		}
		if (Bmi2.IsSupported)
		{
			list.Add("Bmi2");
		}
		if (Fma.IsSupported)
		{
			list.Add("Fma");
		}
		if (Lzcnt.IsSupported)
		{
			list.Add("Lzcnt");
		}
		if (Pclmulqdq.IsSupported)
		{
			list.Add("Pclmulqdq");
		}
		if (Popcnt.IsSupported)
		{
			list.Add("Popcnt");
		}
		if (Sse.IsSupported)
		{
			list.Add("Sse");
		}
		if (Sse2.IsSupported)
		{
			list.Add("Sse2");
		}
		if (Sse3.IsSupported)
		{
			list.Add("Sse3");
		}
		if (Ssse3.IsSupported)
		{
			list.Add("Ssse3");
		}
		if (Sse41.IsSupported)
		{
			list.Add("Sse41");
		}
		if (Sse42.IsSupported)
		{
			list.Add("Sse42");
		}
		if (X86Base.IsSupported)
		{
			list.Add("X86Base");
		}
		if (AdvSimd.IsSupported)
		{
			list.Add("AdvSimd");
		}
		if (System.Runtime.Intrinsics.Arm.Aes.IsSupported)
		{
			list.Add("ArmAes");
		}
		if (ArmBase.IsSupported)
		{
			list.Add("ArmBase");
		}
		if (Crc32.IsSupported)
		{
			list.Add("Crc32");
		}
		if (Dp.IsSupported)
		{
			list.Add("Dp");
		}
		if (Rdm.IsSupported)
		{
			list.Add("Rdm");
		}
		if (Sha1.IsSupported)
		{
			list.Add("Sha1");
		}
		if (Sha256.IsSupported)
		{
			list.Add("Sha256");
		}
		if (PackedSimd.IsSupported)
		{
			list.Add("PackedSimd");
		}
		return list;
	}
}
