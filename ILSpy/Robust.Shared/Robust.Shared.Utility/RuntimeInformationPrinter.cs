using System;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.InteropServices;

namespace Robust.Shared.Utility;

internal static class RuntimeInformationPrinter
{
	public static string[] GetInformationDump()
	{
		Version version = typeof(RuntimeInformationPrinter).Assembly.GetName().Version;
		return new string[9]
		{
			$"OS: {RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture}",
			".NET Runtime: " + RuntimeInformation.FrameworkDescription + " " + RuntimeInformation.RuntimeIdentifier,
			$"Server GC: {GCSettings.IsServerGC}",
			$"Processor: {Environment.ProcessorCount}x {SystemInformation.GetProcessorModel()}",
			"Available Memory: " + ByteHelpers.FormatBytes(GC.GetGCMemoryInfo().TotalAvailableMemoryBytes),
			$"Architecture: {RuntimeInformation.ProcessArchitecture}",
			$"Robust Version: {version}",
			"Compile Options: " + string.Join(';', GetCompileOptions()),
			"Intrinsics: " + string.Join(';', SystemInformation.GetIntrinsics())
		};
	}

	private static List<string> GetCompileOptions()
	{
		return new List<string> { "FULL_RELEASE", "RELEASE", "EXCEPTION_TOLERANCE" };
	}
}
